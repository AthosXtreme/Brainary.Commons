namespace Brainary.Commons.Web.Mvc.Extensions
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Web.Configuration;
    using System.Web.Mvc;

    public static partial class Extensions
    {
        public static bool IsUserInRole(this HtmlHelper helper, params string[] roles)
        {
            if (Convert.ToBoolean(WebConfigurationManager.AppSettings["DisableAuthorization"]))
                return true;

            var user = Thread.CurrentPrincipal;
            if (user == null || !user.Identity.IsAuthenticated)
                return false;

            if (roles.Length > 0 && !roles.Any(user.IsInRole))
                return false;

            return true;
        }

        public static string RenderPartialView(this Controller controller, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}