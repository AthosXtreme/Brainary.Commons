namespace Brainary.Commons.Web.DataTables
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Linq;

    using Brainary.Commons.Extensions;

    public static class DataTablesHelper
    {
        public static DataTableViewModel CreateViewModel<T>(IEnumerable<T> initialData)
        {
            return CreateViewModel(string.Format("DT_{0}", Guid.NewGuid()), null, initialData, t => t, null);
        }

        public static DataTableViewModel CreateViewModel<T>(string id, string ajaxUrl, IEnumerable<T> initialData)
        {
            return CreateViewModel(id, ajaxUrl, initialData, t => t, null);
        }

        public static DataTableViewModel CreateViewModel<T>(string id, string ajaxUrl, IEnumerable<T> initialData, IList<ColDef> columns)
        {
            return CreateViewModel(id, ajaxUrl, initialData, t => t, columns);
        }

        public static DataTableViewModel CreateViewModel<T, TRes>(string id, string ajaxUrl, IEnumerable<T> initialData, Func<T, TRes> transform, IList<ColDef> columns)
        {
            if (columns == null || !columns.Any()) columns = GetColumns(typeof(TRes));
            return new DataTableViewModel<T, TRes>(id, ajaxUrl, initialData, transform, columns);
        }

        public static DataTableViewModel CreateViewModel(string id, string ajaxUrl, DataTable initialData)
        {
            return new DataTableViewModel(id, ajaxUrl, initialData);
        }

        public static DataTableViewModel CreateViewModel(string id, string ajaxUrl, params string[] columns)
        {
            return new DataTableViewModel(id, ajaxUrl, columns.Select(c => ColDef.Create(c, null, typeof(string))).ToList());
        }

        public static IList<ColDef> GetColumns(Type type)
        {
            var propInfos = type.GetSortedProperties();
            return (from propertyInfo in propInfos
                    let displayAttribute = (DisplayAttribute)propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault()
                    let displayName = displayAttribute == null ? propertyInfo.Name : displayAttribute.Name
                    select ColDef.Create(propertyInfo.Name, displayName, propertyInfo.PropertyType)).ToArray();
        }
    }
}
