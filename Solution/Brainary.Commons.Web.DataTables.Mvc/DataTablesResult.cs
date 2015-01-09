namespace Brainary.Commons.Web.DataTables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;

    using Brainary.Commons.Extensions;

    public class DataTablesResult : JsonResult
    {
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
        public DataTablesResult(IQueryable<T> q, RequestParameters dataTableParam, Func<T, TRes> transform, int? totalRecords = null, int? totalDisplay = null)
        {
            var content = DataTablesParser.GetResults(q, dataTableParam, transform);
            if (totalRecords.HasValue) content.recordsTotal = totalRecords.Value;
            if (totalDisplay.HasValue) content.recordsFiltered = totalDisplay.Value;

            Data = content;
            JsonRequestBehavior = JsonRequestBehavior.DenyGet;
        }

        public DataTablesResult(DataTable dt, int totalRecords, int totalDisplay, RequestParameters dataTableParam)
        {
            var content = DataTablesParser.GetResultsDynamic(dt, totalRecords, totalDisplay, dataTableParam);
            Data = content;
            JsonRequestBehavior = JsonRequestBehavior.DenyGet;
        }

        public static void RegisterFilter<TVal>(Func<TVal, object> filter)
        {
            DataTablesParser.PropertyTransformers.Add(DataTablesParser.Guard(filter));
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
            return filteredData.Select(i => properties.Select(p => DataTablesParser.GetTransformedValue(p.PropertyType, p.GetGetMethod().Invoke(i, null))).ToArray()).ToArray();
        }
    }
}