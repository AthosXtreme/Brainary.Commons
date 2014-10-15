namespace Brainary.Commons.Web
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// Redirects unauthorized request to custom /Error/Unauthorized action
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class HandleUnauthorizedAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated || filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                base.HandleUnauthorizedRequest(filterContext);
            else
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "Unauthorized" }));
        }
    }
}