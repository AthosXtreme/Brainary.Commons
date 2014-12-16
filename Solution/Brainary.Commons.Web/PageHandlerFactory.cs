namespace Brainary.Commons.Web
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Web;

    using Brainary.Commons;

    public class PageHandlerFactory : System.Web.UI.PageHandlerFactory
    {
        public override IHttpHandler GetHandler(HttpContext cxt, string type, string virtualPath, string path)
        {
            var page = base.GetHandler(cxt, type, virtualPath, path);
            if (page != null) InjectDependencies(page);
            return page;
        }

        private static void InjectDependencies(object page)
        {
            var pageType = page.GetType().BaseType;
            var ctor = GetInjectableCtor(pageType);

            if (ctor == null) return;

            var arguments = (
                from parameter in ctor.GetParameters()
                select Locator.Instance.Resolve(parameter.ParameterType))
                .ToArray();

            ctor.Invoke(page, arguments);
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

            throw new Exception(string.Format(Commons.Messages.CannotInitializeMultiPublicCtors, type));
        }
    }
}
