namespace Brainary.Commons.Web
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Script.Serialization;
    using System.Web.SessionState;

    using Brainary.Commons.Web.Annotations;

    public abstract class HttpHandler : IHttpHandler, IRequiresSessionState
    {
        public HttpContext Context { get; private set; }

        /// <summary>
        /// Setting this to false will make the handler to respond with exacly what the called method returned.
        /// If true the handler will try to serialize the content based on the ContentType set.
        /// </summary>
        public bool SkipDefaultSerialization { get; set; }

        /// <summary>
        /// Setting this to true will avoid the handler to change the content type wither to its default value or to its specified value on the request.
        /// This is useful if you're handling the request yourself and need to specify it yourself.
        /// </summary>
        public bool SkipContentTypeEvaluation { get; set; }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public virtual object Get()
        {
            return "Default GET Response";
        }

        public virtual object Post()
        {
            return "Default POST Response";
        }

        public virtual object Put()
        {
            return "Default PUT Response";
        }

        public virtual object Delete()
        {
            return "Default DELETE Response";
        }

        /// <summary>
        /// Intercept the execution right before the handler method is called
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnMethodInvoke(MethodInvokeEventArgs e)
        {
        }

        /// <summary>
        /// Intercept the execution right after the handler method is called
        /// </summary>
        public virtual void AfterMethodInvoke(object result)
        {
        }

        /// <summary>
        /// Method used to handle the request as a normal ASHX.
        /// To use this method just pass handlerequest=true on the request query string.
        /// </summary>
        public virtual void HandleRequest()
        {
        }

        public void ProcessRequest(HttpContext context)
        {
            Context = context;

            // it's possible to the requestor to be able to handle everything himself, overriding all this implemention
            var handleRequest = context.Request["handlerequest"];
            if (!string.IsNullOrEmpty(handleRequest) && handleRequest.ToLower() == "true")
            {
                HandleRequest();
                return;
            }

            var ajaxCall = new AjaxCallSignature(context);

            // context.Response.ContentType = string.Empty;
            if (!string.IsNullOrEmpty(ajaxCall.ReturnType))
            {
                switch (ajaxCall.ReturnType)
                {
                    case "json":
                        context.Response.ContentType = ResponseContentTypes.JSON;
                        break;
                    case "xml":
                        context.Response.ContentType = ResponseContentTypes.XML;
                        break;
                    case "jpg":
                    case "jpeg":
                    case "image/jpg":
                        context.Response.ContentType = ResponseContentTypes.ImageJPG;
                        break;
                }
            }

            // call the requested method
            var result = ajaxCall.Invoke(this, context);

            // if neither on the arguments or the actual method the content type was set then make sure to use the default content type
            if (string.IsNullOrEmpty(context.Response.ContentType) && !SkipContentTypeEvaluation)
                context.Response.ContentType = DefaultContentType();

            // only skip transformations if the requestor explicitly said so
            if (result == null)
                context.Response.Write(string.Empty);
            else if (!SkipDefaultSerialization)
                switch (context.Response.ContentType.ToLower())
                {
                    case ResponseContentTypes.JSON:
                        var jsonSerializer = new JavaScriptSerializer();
                        var json = jsonSerializer.Serialize(result);
                        context.Response.Write(json);
                        break;
                    case ResponseContentTypes.XML:
                        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
                        var xmlSb = new StringBuilder();
                        var xmlWriter = System.Xml.XmlWriter.Create(xmlSb);
                        xmlSerializer.Serialize(xmlWriter, result);
                        context.Response.Write(xmlSb.ToString());
                        break;
                    case ResponseContentTypes.HTML:
                        context.Response.Write(result);
                        break;
                    default:
                        throw new Exception(string.Format("Unsuported content type [{0}]", context.Response.ContentType));
                }
            else
                context.Response.Write(result);
        }

        /// <summary>
        /// Returns the default content type returned by the handler.
        /// </summary>
        /// <returns></returns>
        public virtual string DefaultContentType()
        {
            return ResponseContentTypes.JSON;
        }

        public void SetResponseContentType(string value)
        {
            Context.Response.ContentType = value;
        }

        /// <summary>
        /// Prints an help page discribing the available methods on this handler.
        /// </summary>
        public string Help()
        {
            Context.Response.ContentType = ResponseContentTypes.HTML;

            var sb = new StringBuilder();

            sb.AppendLine("<style>");
            sb.AppendLine(".MainHeader { background-color: FFFFE0; border: 1px dashed red; padding: 0 10 0 10; }");
            sb.AppendLine("h3 { background-color: #DCDCDC; }");
            sb.AppendLine("ul { background-color: #FFFFFF; }");
            sb.AppendLine(".type { color: gray; }");
            sb.AppendLine("</style>");

            sb.AppendLine("<div class='MainHeader'><h2>Handler available methods</h2></div>");

            var type = GetType();
            var methods = type.GetMethods(); // All methods found on this type
            var excludeMethods = type.BaseType != null ? type.BaseType.GetMethods() : null; // methods from the base class are not to be shown

            foreach (var m in methods)
            {
                // do nothing if the current method belongs to the base type.
                // I'm not supporting overrides here, I'm only searching by name, if more than one method with the same name exist they all will be ignored.
                if (excludeMethods != null && excludeMethods.FirstOrDefault(c => c.Name == m.Name) != null) continue;

                // get description (search for System.ComponentModel.DescriptionAtrribute)
                var methodDescription = "<i>No description available</i>";
                foreach (var attr in m.GetCustomAttributes(true).OfType<DescriptionAttribute>())
                    methodDescription = attr.Description;

                // get method arguments
                var parameters = m.GetParameters();

                var requiresAuthentication = false;
                var attrs = m.GetCustomAttributes(typeof(RequireAuthenticationAttribute), true);
                if (attrs.Any()) requiresAuthentication = ((RequireAuthenticationAttribute)attrs[0]).RequireAuthentication;

                sb.AppendLine("<h3>" + m.Name + (requiresAuthentication ? " <span style=\"color:#f00\">[Requires Authentication]</span>" : string.Empty) + "</h3>");
                sb.AppendLine(string.Format("<b>Description: </b><i>{0}</i>", methodDescription));

                sb.AppendLine("<table><tr><td width=\"250px\">");
                sb.AppendLine("<table width=\"100%\">");

                foreach (var p in parameters)
                    sb.AppendLine("<tr><td>" + p.Name + "</td><td><span class='type'>[" + p.ParameterType + "]</span></td></tr>");

                sb.AppendLine("</table>");

                sb.AppendLine("</td><td style=\"border-left: 1px dashed #DCDCDC; padding-left: 8px;\">");

                var getJsonSample = "<pre>$.getJSON(\n\t'" + Context.Request.Url.LocalPath + "', \n\t{method: \"" + m.Name + "\", returntype: \"json\", args: {";

                getJsonSample = m.GetParameters().Aggregate(getJsonSample, (current, p) => current + (" " + p.Name + ": \"\","));
                getJsonSample = getJsonSample.TrimEnd(',') + " ";
                getJsonSample += "}}, \n\tfunction() { alert('Success!'); });</pre>";
                sb.AppendLine(getJsonSample);

                sb.AppendLine("</td>");
                sb.AppendLine("</tr></table>");
            }

            return sb.ToString();
        }
    }
}
