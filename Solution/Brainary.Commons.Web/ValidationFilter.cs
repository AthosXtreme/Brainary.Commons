using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Brainary.Commons.Web
{
    /// <summary>
    /// Returns UnprocessableEntityObjectResult (422) for invalid ModelState.
    /// This is a replacement for the default behavior returning incorrect status 400,
    /// which must be deactivated via SuppressModelStateInvalidFilter.
    /// </summary>
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new UnprocessableEntityObjectResult(context.ModelState);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}