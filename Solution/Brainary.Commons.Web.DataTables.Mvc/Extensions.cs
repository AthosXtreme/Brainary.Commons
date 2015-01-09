namespace Brainary.Commons.Web.DataTables
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Mvc;

    using Brainary.Commons.Extensions;

    public static class Extensions
    {
        public static DataTableViewModel DataTableViewModel<TController, TResult>(this HtmlHelper html, string id, Expression<Func<TController, DataTablesResult<TResult>>> action, IList<ColDef> columns = null)
        {
            if (columns == null || !columns.Any()) columns = DataTablesHelper.GetColumns(typeof(TResult));
            var mi = action.MethodInfo();
            var controllerName = typeof(TController).Name;
            controllerName = controllerName.Substring(0, controllerName.LastIndexOf("Controller", StringComparison.Ordinal));
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            var ajaxUrl = urlHelper.Action(mi.Name, controllerName);
            return new DataTableViewModel(id, ajaxUrl, columns);
        }

        public static DataTableViewModel DataTableViewModel<T>(this HtmlHelper html, string id, string ajaxUrl, IQueryable<T> initialData, IList<ColDef> columns = null)
        {
            return DataTableViewModel(html, id, ajaxUrl, initialData, t => t, columns);
        }

        public static DataTableViewModel DataTableViewModel<T, TRes>(this HtmlHelper html, string id, string ajaxUrl, IQueryable<T> initialData, Func<T, TRes> transform, IList<ColDef> columns = null)
        {
            if (columns == null || !columns.Any()) columns = DataTablesHelper.GetColumns(typeof(TRes));
            return new DataTableViewModel<T, TRes>(id, ajaxUrl, initialData, transform, columns);
        }

        public static DataTableViewModel DataTableViewModel(this HtmlHelper html, string id, string ajaxUrl, DataTable initialData)
        {
            return new DataTableViewModel(id, ajaxUrl, initialData);
        }

        public static DataTableViewModel DataTableViewModel(this HtmlHelper html, string id, string ajaxUrl, params string[] columns)
        {
            return new DataTableViewModel(id, ajaxUrl, columns.Select(c => ColDef.Create(c, null, typeof(string))).ToList());
        }
    }
}