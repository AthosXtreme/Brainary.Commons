namespace Brainary.Commons.Web.Datatables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    using Brainary.Commons.Extensions;

    public class DataTablesResult : JsonResult
    {
        protected static readonly List<PropertyTransformer> PropertyTransformers = new List<PropertyTransformer>
        {
            Guard<DateTimeOffset>(dateTimeOffset => dateTimeOffset.DateTime),
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

        public delegate object GuardedValueTransformer<in TVal>(TVal value);

        public delegate object PropertyTransformer(Type type, object value);

        public static DataTablesResult<TRes> Create<T, TRes>(IQueryable<T> q, RequestParameters dataTableParam, Func<T, TRes> transform, int? totalRecords = null, int? totalDisplay = null)
        {
            return new DataTablesResult<T, TRes>(q, dataTableParam, transform, totalRecords, totalDisplay);
        }

        public static DataTablesResult<T> Create<T>(IQueryable<T> q, RequestParameters dataTableParam, int? totalRecords = null, int? totalDisplay = null)
        {
            return new DataTablesResult<T, T>(q, dataTableParam, t => t, totalRecords, totalDisplay);
        }

        public static DataTablesResult Create(object queryable, RequestParameters dataTableParam)
        {
            queryable = ((IEnumerable)queryable).AsQueryable();
            const string S = "Create";

            var openCreateMethod =
                typeof(DataTablesResult).GetMethods().Single(x => x.Name == S && x.GetGenericArguments().Count() == 1);
            var queryableType = queryable.GetType().GetGenericArguments()[0];
            var closedCreateMethod = openCreateMethod.MakeGenericMethod(queryableType);
            return (DataTablesResult)closedCreateMethod.Invoke(null, new[] { queryable, dataTableParam });
        }

        public static DataTablesResult CreateDynamic(DataTable dt, int totalRecords, int totalDisplay, RequestParameters dataTableParam)
        {
            return new DataTablesResult<object, object>(dt, totalRecords, totalDisplay, dataTableParam);
        }

        public static DataTablesResult<T> CreateResultUsingEnumerable<T>(IEnumerable<T> q, RequestParameters dataTableParam, int? totalRecords = null, int? totalDisplay = null)
        {
            return new DataTablesResult<T, T>(q.AsQueryable(), dataTableParam, t => t, totalRecords, totalDisplay);
        }

        internal static object GetTransformedValue(Type propertyType, object value)
        {
            foreach (var result in PropertyTransformers.Select(transformer => transformer(propertyType, value)).Where(result => result != null)) return result;
            return (value ?? string.Empty).ToString();
        }

        protected static PropertyTransformer Guard<TVal>(GuardedValueTransformer<TVal> transformer)
        {
            return (t, v) => typeof(TVal).IsAssignableFrom(t) ? v != DBNull.Value ? transformer((TVal)v) : null : null;
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    // ReSharper disable once UnusedTypeParameter
    public class DataTablesResult<T> : DataTablesResult
    {
        public static object[] ParseData(T data)
        {
            var foo = new List<T> { data };
            return ParseData(foo.AsQueryable()).First();
        }

        public static object[][] ParseData(IQueryable<T> data)
        {
            return DataTablesResult<T, T>.ParseData(data, t => t);
        }
    }

    public class DataTablesResult<T, TRes> : DataTablesResult<TRes>
    {
        private readonly Func<T, TRes> transform;
        
        public DataTablesResult(IQueryable<T> q, RequestParameters dataTableParam, Func<T, TRes> transform, int? totalRecords = null, int? totalDisplay = null)
        {
            this.transform = transform;
             
            var content = GetResults(q, dataTableParam);
            if (totalRecords.HasValue) content.recordsTotal = totalRecords.Value;
            if (totalDisplay.HasValue) content.recordsFiltered = totalDisplay.Value;

            Data = content;
            JsonRequestBehavior = JsonRequestBehavior.DenyGet;
        }

        public DataTablesResult(DataTable dt, int totalRecords, int totalDisplay, RequestParameters dataTableParam)
        {
            var content = GetResultsDynamic(dt, totalRecords, totalDisplay, dataTableParam);
            Data = content;
            JsonRequestBehavior = JsonRequestBehavior.DenyGet;
        }

        public static void RegisterFilter<TVal>(GuardedValueTransformer<TVal> filter)
        {
            PropertyTransformers.Add(Guard(filter));
        }

        public static object[] ParseData(T data, Func<T, TRes> transform)
        {
            var foo = new List<T> { data };
            return ParseData(foo.AsQueryable(), transform).First();
        }

        public static object[][] ParseData(IQueryable<T> data, Func<T, TRes> transform)
        {
            var filteredData = data.Select(transform).AsQueryable();
            var properties = typeof(TRes).GetSortedProperties();
            return filteredData.Select(i => properties.Select(p => GetTransformedValue(p.PropertyType, p.GetGetMethod().Invoke(i, null))).ToArray()).ToArray();
        }

        private DataTablesResponse GetResults(IQueryable<T> data, RequestParameters param)
        {
            var totalRecords = data.Count(); // annoying this, as it causes an extra evaluation..

            var filters = new DataTablesFilter();

            var filteredData = data.Select(transform).AsQueryable();

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

        private DataTablesResponse GetResultsDynamic(DataTable data, int totalRecords, int totalDisplay, RequestParameters param)
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
    }

    public class ColInfo
    {
        public ColInfo(string name, Type propertyType)
        {
            Name = name;
            Type = propertyType;
        }

        public string Name { get; set; }

        public Type Type { get; set; }
    }
}