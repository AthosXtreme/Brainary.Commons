namespace Brainary.Commons.Web.DataTables
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;

    using Brainary.Commons.Extensions;

    public class DataTablesParser
    {
        static DataTablesParser()
        {
            PropertyTransformers = new List<Func<Type, object, object>>
            {
                Guard<DateTimeOffset>(s => s.DateTime),
                Guard<IHtmlString>(s => s.ToHtmlString()),
                Guard<IEnumerable<string>>(s => s.ToArray()),
                Guard<IEnumerable<int>>(s => s.ToArray()),
                Guard<IEnumerable<long>>(s => s.ToArray()),
                Guard<IEnumerable<decimal>>(s => s.ToArray()),
                Guard<IEnumerable<bool>>(s => s.ToArray()),
                Guard<IEnumerable<double>>(s => s.ToArray()),
                Guard<IEnumerable<object>>(s => s.Select(o => GetTransformedValue(o.GetType(), o)).ToArray()),
                Guard<DateTime>(s => s),
                Guard<int>(s => s),
                Guard<long>(s => s),
                Guard<decimal>(s => s),
                Guard<double>(s => s),
                Guard<DateTime?>(s => s),
                Guard<int?>(s => s),
                Guard<long?>(s => s),
                Guard<decimal?>(s => s),
                Guard<double?>(s => s),
                Guard<bool>(s => s)
            };
        }

        public static IList<Func<Type, object, object>> PropertyTransformers { get; set; }

        public static object GetTransformedValue(Type propertyType, object value)
        {
            foreach (var result in PropertyTransformers.Select(transformer => transformer(propertyType, value)).Where(result => result != null)) return result;
            return (value ?? string.Empty).ToString();
        }

        public static Func<Type, object, object> Guard<TVal>(Func<TVal, object> transformer)
        {
            return (t, v) => typeof(TVal).IsAssignableFrom(t) ? v != null && v != DBNull.Value ? transformer((TVal)v) : null : null;
        }

        public static DataTablesResponse GetResultsDynamic(DataTable data, int totalRecords, int totalDisplay, RequestParameters param)
        {
            var filteredData = data.Rows.Cast<DataRow>().Select(s => s.ItemArray).ToArray();

            var transformedPage = filteredData.Select(s => s.Select((v, i) => GetTransformedValue(data.Columns[i].DataType, v)).ToArray()).ToArray();

            var result = new DataTablesResponse
            {
                recordsTotal = totalRecords,
                recordsFiltered = totalDisplay,
                draw = param.Draw,
                data = transformedPage
            };

            return result;
        }

        public static DataTablesResponse GetResults<T, TRes>(IEnumerable<T> data, RequestParameters param, Func<T, TRes> transform)
        {
            var list = data.ToList();
            var totalRecords = list.Count();
            var filters = new DataTablesFilter();
            var filteredData = list.Select(transform).AsQueryable();
            var searchColumns = typeof(TRes).GetSortedProperties().Select(p => new ColInfo(p.Name, p.PropertyType)).ToArray();

            filteredData = filters.FilterPagingSortingSearch(param, filteredData, searchColumns).Cast<TRes>();

            var page = filteredData.Skip(param.Start);
            if (param.Length > -1)
                page = page.Take(param.Length);

            var properties = typeof(TRes).GetSortedProperties();

            var transformedPage = page.Select(i => properties.Select(p => GetTransformedValue(p.PropertyType, p.GetGetMethod().Invoke(i, null))).ToArray()).ToArray();

            var result = new DataTablesResponse
            {
                recordsTotal = totalRecords,
                recordsFiltered = filteredData.Count(),
                draw = param.Draw,
                data = transformedPage
            };

            return result;
        }
    }
    
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class DataTablesParser<T> : DataTablesParser
    {
        public static object[] ParseData(T data)
        {
            var foo = new List<T> { data };
            return ParseData(foo.AsQueryable()).First();
        }

        public static object[][] ParseData(IEnumerable<T> data)
        {
            return DataTablesParser<T, T>.ParseData(data, t => t);
        }
    }

    public class DataTablesParser<T, TRes> : DataTablesParser<TRes>
    {
        public static void RegisterFilter<TVal>(Func<TVal, object> filter)
        {
            PropertyTransformers.Add(Guard(filter));
        }

        public static object[] ParseData(T data, Func<T, TRes> transform)
        {
            var foo = new List<T> { data };
            return ParseData(foo.AsQueryable(), transform).First();
        }

        public static object[][] ParseData(IEnumerable<T> data, Func<T, TRes> transform)
        {
            var filteredData = data.Select(transform);
            var properties = typeof(TRes).GetSortedProperties();
            return filteredData.Select(i => properties.Select(p => GetTransformedValue(p.PropertyType, p.GetGetMethod().Invoke(i, null))).ToArray()).ToArray();
        }
    }
}