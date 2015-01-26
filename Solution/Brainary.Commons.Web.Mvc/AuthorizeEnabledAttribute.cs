namespace Brainary.Commons.Web.Mvc
{
    using System;
    using System.Web.Configuration;
    using System.Web.Mvc;

    /// <summary>
    /// Avoid authorization if DisableAuthorization appsetting is true
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AuthorizeEnabledAttribute : HandleUnauthorizedAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Convert.ToBoolean(WebConfigurationManager.AppSettings["DisableAuthorization"])) return;
            base.OnAuthorization(filterContext);
        }
    }
}