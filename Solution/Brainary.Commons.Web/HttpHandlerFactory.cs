namespace Brainary.Commons.Web
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web;

    public sealed class HttpHandlerFactory : IHttpHandlerFactory
    {
        private static readonly IDictionary<string, ConstructorInfo> CtorList;
        private static readonly object SyncLock;

        static HttpHandlerFactory()
        {
            CtorList = new Dictionary<string, ConstructorInfo>();
            SyncLock = 0;
        }

        public IHttpHandler GetHandler(HttpContext cxt, string type, string virtualPath, string path)
        {
            var fname = string.Format("{0}", Path.GetFileNameWithoutExtension(virtualPath)).ToLower();
            ConstructorInfo ctor;

            lock (SyncLock)
            {
                if (!CtorList.ContainsKey(fname))
                {
                    Type pageType = null;
                    var baseType = cxt.ApplicationInstance.GetType().BaseType;
                    if (baseType != null) pageType = baseType.Assembly.GetTypes().FirstOrDefault(f => f.Name.ToLower() == fname);

                    if (pageType == null)
                    {
                        cxt.Response.StatusCode = 404;
                        cxt.ApplicationInstance.CompleteRequest();
                        return null;
                    }

                    ctor = GetInjectableCtor(pageType) ?? pageType.GetConstructor(new Type[] { });
                    CtorList.Add(fname, ctor);
                }
            }

            ctor = CtorList[fname];
            var handler = InjectDependencies(ctor);

            if (handler is HttpHandler) return handler;

            cxt.Response.StatusCode = 404;
            return null;
        }

        public void ReleaseHandler(IHttpHandler handler)
        {
        }

        private static IHttpHandler InjectDependencies(ConstructorInfo ctor)
        {
            var arguments = GetResolvedArguments(ctor);
            return ctor.Invoke(arguments) as IHttpHandler;
        }

        private static ConstructorInfo GetInjectableCtor(Type type)
        {
            var overloadedPublicConstructors = (
                from constructor in type.GetConstructors()
                where constructor.GetParameters().Length > 0
                select constructor).ToArray();

            if (overloadedPublicConstructors.Length == 0)
                return null;

            if (overloadedPublicConstructors.Length == 1)
                return overloadedPublicConstructors[0];

            throw new Exception(string.Format(Messages.CannotInitializeMultiPublicCtors, type));
        }

        private static object[] GetResolvedArguments(ConstructorInfo ctor)
        {
            return (from parameter in ctor.GetParameters() select Locator.Instance.Resolve(parameter.ParameterType)).ToArray();
        }
    }
}
