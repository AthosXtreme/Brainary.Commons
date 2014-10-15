namespace Brainary.Commons.Web
{
    using System;
    using System.Web;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class CacheControlAttribute : ActionFilterAttribute
    {
        public CacheControlAttribute(HttpCacheability cacheability)
        {
            Cacheability = cacheability;
        }

        public HttpCacheability Cacheability { get; private set; }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var cache = filterContext.HttpContext.Response.Cache;
            cache.SetCacheability(Cacheability);
        }
    }
}
