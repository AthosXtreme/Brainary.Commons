namespace Brainary.Commons.Web
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.Script.Serialization;

    using Brainary.Commons.Web.Annotations;

    public class AjaxCallSignature
    {
        public AjaxCallSignature(HttpContext context)
        {
            Args = new Dictionary<string, object>();
            Method = string.Empty;
            var nullKeyParameter = context.Request.QueryString[null];

            if (new[] { "POST", "PUT", "DELETE" }.Contains(context.Request.RequestType.ToUpper()))
            {
                var requestParams = context.Request.Params.AllKeys;
                foreach (var item in requestParams)
                {
                    switch (item.ToLower())
                    {
                        case "method":
                            Method = context.Request.Params[item];
                            break;
                        case "returntype":
                            ReturnType = context.Request.Params[item];
                            break;
                        default:
                            if (item.ToLower().StartsWith("args["))
                            {
                                var key = item.Trim().TrimEnd(']').Substring(5);
                                key = key.Trim().Replace("][", "+");

                                var value = context.Request.Params[item];
                                Args.Add(key, value);
                            }
                            else
                            {
                                var key = item;
                                var value = context.Request.Params[item];
                                Args.Add(key, value);
                            }

                            break;
                    }
                }
            }
            else if (context.Request.RequestType.ToUpper() == "GET")
            {
                // evaluate the data passed as json
                if (!string.IsNullOrEmpty(nullKeyParameter))
                {
                    if (nullKeyParameter.ToLower() == "help")
                    {
                        Method = "help";
                        return;
                    }

                    var serializer = new JavaScriptSerializer();
                    var json = serializer.DeserializeObject(context.Request.QueryString[null]);

                    try
                    {
                        var dict = (IDictionary<string, object>)json;

                        if (dict.ContainsKey("method"))
                            Method = dict["method"].ToString();
                        else
                            throw new Exception("Invalid BaseHandler call. MethodName parameter is mandatory in json object.");

                        if (dict.ContainsKey("returntype"))
                            ReturnType = dict["returntype"].ToString();

                        if (dict.ContainsKey("args"))
                            Args = (IDictionary<string, object>)dict["args"];
                        else
                            Args = new Dictionary<string, object>();
                    }
                    catch
                    {
                        throw new InvalidCastException("Unable to cast json object to AjaxCallSignature");
                    }
                }

                // evaluate data passed as querystring params
                foreach (var key in context.Request.QueryString.Keys.Cast<string>().Where(key => key != null))
                {
                    switch (key.ToLower())
                    {
                        case "method":
                            if (string.IsNullOrEmpty(Method))
                                Method = context.Request.QueryString[key];
                            else
                                throw new Exception("Method name was already specified on the json data. Specify the method name only once, either on QueryString params or on the json data.");
                            break;
                        case "returntype":
                            ReturnType = context.Request.QueryString[key];
                            break;
                        default:
                            if (key.ToLower().StartsWith("args["))
                            {
                                var k = key.Trim().Substring(5).TrimEnd(']').Replace("][", "+");
                                Args.Add(k, context.Request.QueryString[key]);
                            }
                            else Args.Add(key, context.Request.QueryString[key]);
                            break;
                    }
                }
            }
        }

        public string Method { get; set; }

        public string ReturnType { get; set; }

        public IDictionary<string, object> Args { get; set; }

        public object Invoke(HttpHandler handler, HttpContext context)
        {
            // call the request method
            // if no method is passed then well call the method by HTTP verb (GET, POST, DELETE, UPDATE)
            if (string.IsNullOrEmpty(Method)) Method = context.Request.RequestType.ToUpper();

            var t = handler.GetType();
            var m = t.GetMethod(Method);

            if (m == null)
            {
                if (Method.ToLower() == "help" && t.BaseType != null)
                    m = t.BaseType.GetMethod("Help");
                else
                    throw new Exception(string.Format("Method {0} not found on Handler {1}.", Method, GetType()));
            }
            else
            {
                // evaluate the handler and method attributes against Http allowed verbs
                /* The logic here is:
                *  -> if no attribute is found means it allows every verb
                *  -> if a method have verb attbibutes defined then it will ignore the ones on the class
                *  -> verb attributes on the class are applied to all methods without verb attribues
                */
                var handlerSupportedVerbs = handler.GetType().GetCustomAttributes(typeof(HttpVerbAttribute), true).Cast<HttpVerbAttribute>().ToList();
                var methodSupportedVerbs = m.GetCustomAttributes(typeof(HttpVerbAttribute), true).Cast<HttpVerbAttribute>().ToList();

                var verbAllowedOnMethod = !methodSupportedVerbs.Any();
                var verbAllowedOnHandler = !handlerSupportedVerbs.Any();
                if (methodSupportedVerbs.Any())
                    verbAllowedOnMethod = methodSupportedVerbs.FirstOrDefault(x => x.HttpVerb == context.Request.RequestType.ToUpper()) != null;
                else if (handlerSupportedVerbs.Any())
                    verbAllowedOnHandler = handlerSupportedVerbs.FirstOrDefault(x => x.HttpVerb == context.Request.RequestType.ToUpper()) != null;

                if (!verbAllowedOnMethod || !verbAllowedOnHandler) throw new HttpVerbNotAllowedException(Commons.Messages.HTTPVerbNotAllowed);

                // security validation: Search for RequireAuthenticationAttribute on the method
                //     value=true the user must be authenticated (only supports FromsAuthentication for now
                //     value=false invoke the method
                var attrs = m.GetCustomAttributes(typeof(RequireAuthenticationAttribute), true);
                if (attrs.Any())
                {
                    if (!context.Request.IsAuthenticated && ((RequireAuthenticationAttribute)attrs[0]).RequireAuthentication)
                        throw new InvalidOperationException("Method [" + m.Name + "] Requires authentication");
                }
            }

            // OnMethodInvoke -> Invoke -> AfterMethodInvoke
            var cancelInvoke = new MethodInvokeEventArgs(m);
            handler.OnMethodInvoke(cancelInvoke);

            if (cancelInvoke.Cancel) return null;

            var invokeResult = m.Invoke(handler, m.GetParameters().Select(param => ProcessProperty(param.Name, param.ParameterType, string.Empty)).ToArray());
            handler.AfterMethodInvoke(invokeResult);

            return invokeResult;
        }

        /// <summary>
        /// Hydrates CLR primitive types
        /// </summary>
        public object HydrateValue(string propertyName, Type propertyType, string parentNamespace)
        {
            var propFqn = string.IsNullOrEmpty(parentNamespace) ? propertyName : parentNamespace + "+" + propertyName;
            if (!Args.Keys.Contains(propFqn)) return null; // if there are missing arguments try passing null

            // its usual to pass an empty json string property but casting it to certain types will throw an exception
            if (string.IsNullOrEmpty(Args[propFqn].ToString()) || Args[propFqn].ToString() == "null" || Args[propFqn].ToString() == "undefined")
            {
                // handle numerics. convert null or empty input values to 0
                if (propertyType == typeof(short) || propertyType == typeof(int) || propertyType == typeof(long) || propertyType == typeof(decimal) || propertyType == typeof(double) || propertyType == typeof(byte))
                    Args[propFqn] = 0;
                else if (propertyType == typeof(Guid))
                    Args[propFqn] = new Guid();
                else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    Args[propFqn] = null;
            }

            // evaluate special types that are not directly casted from string
            var conv = TypeDescriptor.GetConverter(propertyType);
            if (Args[propFqn] == null || propertyType == Args[propFqn].GetType())
                return Args[propFqn];
                
            return conv.ConvertFrom(Args[propFqn]);
        }

        /// <summary>
        /// Hydrates complex types
        /// </summary>
        public object HydrateClass(string propertyName, Type propertyType, string parentNamespace)
        {
            var argumentObject = Activator.CreateInstance(propertyType);

            // search for properties on the current namespace
            var prefix = string.IsNullOrEmpty(parentNamespace) ? propertyName : parentNamespace + "+" + propertyName;

            var objectProperties = Args.Keys.ToList().FindAll(k => k.StartsWith(prefix));

            // loop through them 
            foreach (var propName in objectProperties.Select(p => p.Remove(0, prefix.Length + 1).Split('+')[0]))
            {
                argumentObject.GetType()
                    .GetProperty(propName)
                    .SetValue(argumentObject, ProcessProperty(propName, argumentObject.GetType().GetProperty(propName).PropertyType, prefix), null);
            }

            return argumentObject;
        }

        private object ProcessProperty(string propertyName, Type propertyType, string parentNamespace)
        {
            if (propertyType.IsArray || (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>)))
                return HydrateArray(propertyName, propertyType, parentNamespace);

            if (propertyType.IsClass && !(propertyType == typeof(string)))
                return HydrateClass(propertyName, propertyType, parentNamespace);

            return HydrateValue(propertyName, propertyType, parentNamespace);
        }

        private object HydrateArray(string propertyName, Type propertyType, string parentNamespace)
        {
            Array result;
            var elementType = propertyType.IsGenericType ? propertyType.GetGenericArguments()[0] : propertyType.GetElementType();

            var propFqn = string.IsNullOrEmpty(parentNamespace) ? propertyName : parentNamespace + "+" + propertyName;

            if (elementType.IsValueType)
            {
                var conv = TypeDescriptor.GetConverter(elementType);
                var values = Args[propFqn + "+"].ToString().Split(new[] { ',' });

                result = Array.CreateInstance(elementType, values.Length);

                for (var i = 0; i < values.Length; i++)
                    result.SetValue(conv.ConvertFromString(values[i]), i);
            }
            else
            {
                // get the properties in the current nesting depth
                var objectProperties = Args.Keys.ToList().FindAll(k => k.StartsWith(propFqn + "+"));

                // get the number of items in the array
                var maxIndex = (from p in objectProperties select p.Remove(0, propFqn.Length + 1) into idx select idx.Substring(0, idx.IndexOf('+')) into idx select Convert.ToInt32(idx)).Concat(new[] { 0 }).Max();

                // create the instance of the array
                result = Array.CreateInstance(elementType, maxIndex + 1);
                for (var i = 0; i <= maxIndex; i++)
                {
                    result.SetValue(ProcessProperty(propertyName + "+" + i.ToString(CultureInfo.InvariantCulture), result.GetType().GetElementType(), parentNamespace), i);
                }
            }

            return propertyType.IsGenericType ? Activator.CreateInstance(propertyType, new object[] { result }) : result;
        }
    }
}
