namespace Brainary.Commons.Web
{
    using System;
    using System.Web.Configuration;
    using System.Web.Mvc;

    /// <summary>
    /// Perform if DisableAuthorization configuration appsetting is false
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