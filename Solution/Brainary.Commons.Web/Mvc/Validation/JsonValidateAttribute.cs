namespace Brainary.Commons.Web.Mvc.Validation
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class JsonValidateAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.Controller.ViewData.ModelState.IsValid) return;

            var validation = filterContext.Controller.ViewData.ModelState.Where(w => w.Value.Errors.Count > 0).Select(s => s.Value.Errors.Select(ss => ss.ErrorMessage).First());
            var jsonResult = new JsonResult { Data = new { Status = false, Message = Messages.InvalidModel, Validation = validation } };
            filterContext.Result = jsonResult;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // Do nothing
        }
    }
}
