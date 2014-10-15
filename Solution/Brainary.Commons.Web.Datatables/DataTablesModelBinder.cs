namespace Brainary.Commons.Web.Datatables
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;

    public class DataTablesModelBinder : IModelBinder
    {
        public virtual object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var obj = new RequestParameters();
            var request = controllerContext.HttpContext.Request.Params;

            obj.Draw = int.Parse(request["draw"]);
            obj.Start = Convert.ToInt32(request["start"]);
            obj.Length = Convert.ToInt32(request["length"]);

            var regexp = new Regex(@"^(customData)(\[)(\w+)(\])$");
            obj.CustomData = request.Cast<string>().Where(w => regexp.IsMatch(w)).Select(k => new { Key = regexp.Replace(k, "$3"), Value = request[k] }).ToDictionary(k => k.Key, v => v.Value);

            return obj;
        }
    }
}