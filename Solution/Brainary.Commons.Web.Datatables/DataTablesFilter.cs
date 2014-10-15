namespace Brainary.Commons.Web.Datatables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Brainary.Commons.Dynamic;

    public class DataTablesFilter
    {
        private static readonly List<ReturnedFilteredQueryForType> Filters;

        static DataTablesFilter()
        {
            Filters = new List<ReturnedFilteredQueryForType>
            {
                Guard(IsBoolType, TypeFilters.BoolFilter),
                Guard(IsDateTimeType, TypeFilters.DateTimeFilter),
                Guard(IsDateTimeOffsetType, TypeFilters.DateTimeOffsetFilter),
                Guard(IsNumericType, TypeFilters.NumericFilter),
                Guard(IsEnumType, TypeFilters.EnumFilter),
                Guard(arg => arg == typeof(string), TypeFilters.StringFilter),
            };
        }

        public delegate string ReturnedFilteredQueryForType(string query, string columnName, Type columnType, List<object> parametersForLinqQuery);

        public delegate string GuardedFilter(string query, string columnName, Type columnType, List<object> parametersForLinqQuery);

        public static void RegisterFilter<T>(GuardedFilter filter)
        {
            Filters.Add(Guard(arg => arg is T, filter));
        }

        public static bool IsEnumType(Type type)
        {
            return type.IsEnum;
        }

        public static bool IsBoolType(Type type)
        {
            return type == typeof(bool) || type == typeof(bool?);
        }

        public static bool IsDateTimeType(Type type)
        {
            return type == typeof(DateTime) || type == typeof(DateTime?);
        }

        public static bool IsDateTimeOffsetType(Type type)
        {
            return type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?);
        }

        public static bool IsNumericType(Type type)
        {
            if (type == null || type.IsEnum)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumericType(Nullable.GetUnderlyingType(type));
                    }

                    return false;
            }

            return false;
        }

        public IQueryable FilterPagingSortingSearch(RequestParameters tableParameters, IQueryable data, ColInfo[] columns)
        {
            if (!string.IsNullOrEmpty(tableParameters.Search.Value))
            {
                var parts = new List<string>();
                var parameters = new List<object>();
                foreach (var column in tableParameters.Columns.Where(column => column.Searchable))
                {
                    try
                    {
                        parts.Add(GetFilterClause(tableParameters.Search.Value, columns.First(f => f.Name == column.Name), parameters));
                    }
                    catch
                    {
                        // if the clause doesn't work, skip it!
                    }
                }

                data = data.Where(string.Join(" or ", parts), parameters.ToArray());
            }

            foreach (var column in tableParameters.Columns.Where(column => column.Searchable))
            {
                if (!string.IsNullOrWhiteSpace(column.Search.Value))
                {
                    var parameters = new List<object>();
                    var filterClause = GetFilterClause(column.Search.Value, columns.First(f => f.Name == column.Name), parameters);
                    if (string.IsNullOrWhiteSpace(filterClause) == false)
                    {
                        data = data.Where(filterClause, parameters.ToArray());
                    }
                }
            }

            var sortString = string.Join(", ", tableParameters.Order.Select(s => string.Format("{0} {1}", columns[s.Column].Name, s.Dir)));

            if (string.IsNullOrWhiteSpace(sortString))
            {
                sortString = columns[0].Name;
            }

            data = data.OrderBy(sortString);

            return data;
        }

        private static ReturnedFilteredQueryForType Guard(Func<Type, bool> guard, GuardedFilter filter)
        {
            return (q, c, t, p) => !guard(t) ? null : filter(q, c, t, p);
        }

        private static string GetFilterClause(string query, ColInfo column, List<object> parametersForLinqQuery)
        {
            Func<string, string> filterClause = queryPart =>
                                                Filters.Select(
                                                    f => f(queryPart, column.Name, column.Type, parametersForLinqQuery))
                                                       .First(filterPart => filterPart != null);

            var queryParts = query.Split('|').Select(filterClause).Where(fc => fc != string.Empty).ToArray();
            if (queryParts.Any())
            {
                return "(" + string.Join(") OR (", queryParts) + ")";
            }

            return null;
        }
    }
}