namespace Brainary.Commons.Web.Datatables
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;

    using Brainary.Commons.Extensions;

    public static class DataTablesHelper
    {
        public static DataTableVm DataTableViewModel<TController, TResult>(this HtmlHelper html, string id, Expression<Func<TController, DataTablesResult<TResult>>> exp, IList<ColDef> columns = null)
        {
            if (columns == null || !columns.Any()) columns = GetColumns(typeof(TResult));
            var mi = exp.MethodInfo();
            var controllerName = typeof(TController).Name;
            controllerName = controllerName.Substring(0, controllerName.LastIndexOf("Controller", StringComparison.Ordinal));
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            var ajaxUrl = urlHelper.Action(mi.Name, controllerName);
            return new DataTableVm(id, ajaxUrl, columns);
        }

        public static DataTableVm DataTableViewModel<T>(this HtmlHelper html, string id, string ajaxUrl, IQueryable<T> initialData, IList<ColDef> columns = null)
        {
            return DataTableViewModel(html, id, ajaxUrl, initialData, t => t, columns);
        }

        public static DataTableVm DataTableViewModel<T, TRes>(this HtmlHelper html, string id, string ajaxUrl, IQueryable<T> initialData, Func<T, TRes> transform, IList<ColDef> columns = null)
        {
            if (columns == null || !columns.Any()) columns = GetColumns(typeof(TRes));
            return new DataTableVm<T, TRes>(id, ajaxUrl, initialData, transform, columns);
        }

        public static DataTableVm DataTableViewModel(this HtmlHelper html, string id, string ajaxUrl, DataTable initialData)
        {
            return new DataTableVm(id, ajaxUrl, initialData);
        }

        public static DataTableVm DataTableViewModel(this HtmlHelper html, string id, string ajaxUrl, params string[] columns)
        {
            return new DataTableVm(id, ajaxUrl, columns.Select(c => ColDef.Create(c, null, typeof(string))).ToList());
        }

        private static IList<ColDef> GetColumns(Type type)
        {
            var propInfos = type.GetSortedProperties();
            return (from propertyInfo in propInfos
                       let displayAttribute = (DisplayAttribute)propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault()
                       let displayName = displayAttribute == null ? propertyInfo.Name : displayAttribute.Name
                       select ColDef.Create(propertyInfo.Name, displayName, propertyInfo.PropertyType)).ToArray();
        }
    }
}