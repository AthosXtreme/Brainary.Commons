﻿namespace Brainary.Commons.Web.DataTables
{
    using System;
    using System.Collections.Generic;

    internal static class TypeFilters
    {
        private static readonly Func<string, Type, object> ParseValue =
            (input, t) => t.IsEnum ? Enum.Parse(t, input) : Convert.ChangeType(input, t);

        public static string NumericFilter(string query, string columnname, Type columnType, List<object> parametersForLinqQuery)
        {
            if (query.StartsWith("^")) query = query.TrimStart('^');
            if (query.EndsWith("$")) query = query.TrimEnd('$');

            if (query == "~") return string.Empty;

            if (query.Contains("~"))
            {
                var parts = query.Split('~');
                var clause = null as string;

                try
                {
                    parametersForLinqQuery.Add(Convert.ChangeType(parts[0], columnType));
                    clause = string.Format("{0} >= @{1}", columnname, parametersForLinqQuery.Count - 1);
                }
                catch (FormatException)
                {
                }

                try
                {
                    parametersForLinqQuery.Add(Convert.ChangeType(parts[1], columnType));
                    if (clause != null) clause += " and ";
                    clause += string.Format("{0} <= @{1}", columnname, parametersForLinqQuery.Count - 1);
                }
                catch (FormatException)
                {
                }

                return clause ?? "true";
            }

            try
            {
                parametersForLinqQuery.Add(Convert.ChangeType(query, columnType));
                return string.Format("{0} == @{1}", columnname, parametersForLinqQuery.Count - 1);
            }
            catch (FormatException)
            {
            }

            return "false";
        }

        public static string DateTimeOffsetFilter(string query, string columnname, Type columnType, List<object> parametersForLinqQuery)
        {
            if (query == "~") return string.Empty;

            if (!query.Contains("~"))
                return string.Format("{1}.ToLocalTime().ToString(\"g\").{0}", FilterMethod(query, parametersForLinqQuery, columnType), columnname);

            var parts = query.Split('~');
            DateTimeOffset start, end;
            DateTimeOffset.TryParse(parts[0] ?? string.Empty, out start);
            if (!DateTimeOffset.TryParse(parts[1] ?? string.Empty, out end)) end = DateTimeOffset.MaxValue;

            parametersForLinqQuery.Add(start);
            parametersForLinqQuery.Add(end);
            return string.Format("{0} >= @{1} and {0} <= @{2}", columnname, parametersForLinqQuery.Count - 2, parametersForLinqQuery.Count - 1);
        }

        public static string DateTimeFilter(string query, string columnname, Type columnType, List<object> parametersForLinqQuery)
        {
            if (query == "~") return string.Empty;

            if (!query.Contains("~"))
                return string.Format("{1}.ToLocalTime().ToString(\"g\").{0}", FilterMethod(query, parametersForLinqQuery, columnType), columnname);

            var parts = query.Split('~');
            DateTime start, end;
            DateTime.TryParse(parts[0] ?? string.Empty, out start);
            if (!DateTime.TryParse(parts[1] ?? string.Empty, out end)) end = DateTime.MaxValue;

            parametersForLinqQuery.Add(start);
            parametersForLinqQuery.Add(end);
            return string.Format("{0} >= @{1} and {0} <= @{2}", columnname, parametersForLinqQuery.Count - 2, parametersForLinqQuery.Count - 1);
        }

        public static string BoolFilter(string query, string columnname, Type columnType, List<object> parametersForLinqQuery)
        {
            if (query != null)
                query = query.TrimStart('^').TrimEnd('$');
            if (string.IsNullOrWhiteSpace(query)) return columnname + " == null";
            if (query.ToLower() == "true") return columnname + " == true";
            return columnname + " == false";
        }

        public static string StringFilter(string q, string columnname, Type columntype, List<object> parametersforlinqquery)
        {
            if (q == ".*") return string.Empty;
            if (q.StartsWith("^"))
            {
                if (q.EndsWith("$"))
                {
                    parametersforlinqquery.Add(q.Substring(1, q.Length - 2));
                    var parameterArg = "@" + (parametersforlinqquery.Count - 1);
                    return string.Format("{0} ==  {1}", columnname, parameterArg);
                }
                else
                {
                    parametersforlinqquery.Add(q.Substring(1));
                    var parameterArg = "@" + (parametersforlinqquery.Count - 1);
                    return string.Format("({0} != null && {0} != \"\" && ({0} ==  {1} || {0}.StartsWith({1})))", columnname, parameterArg);
                }
            }

            parametersforlinqquery.Add(q);
            var parameterArgu = "@" + (parametersforlinqquery.Count - 1);
            return string.Format("({0} != null && {0} != \"\" && ({0} ==  {1} || {0}.StartsWith({1}) || {0}.Contains({1})))", columnname, parameterArgu);
        }

        public static string EnumFilter(string q, string columnname, Type type, List<object> parametersForLinqQuery)
        {
            if (q.StartsWith("^")) q = q.Substring(1);
            if (q.EndsWith("$")) q = q.Substring(0, q.Length - 1);
            parametersForLinqQuery.Add(ParseValue(q, type));
            return columnname + " == @" + (parametersForLinqQuery.Count - 1);
        }

        internal static string FilterMethod(string q, List<object> parametersForLinqQuery, Type type)
        {
            Func<string, string, string> makeClause = (method, query) =>
            {
                parametersForLinqQuery.Add(ParseValue(query, type));
                var indexOfParameter = parametersForLinqQuery.Count - 1;
                return string.Format("{0}(@{1})", method, indexOfParameter);
            };

            if (q.StartsWith("^"))
            {
                if (!q.EndsWith("$"))
                    return makeClause("StartsWith", q.Substring(1));

                parametersForLinqQuery.Add(ParseValue(q.Substring(1, q.Length - 2), type));
                var indexOfParameter = parametersForLinqQuery.Count - 1;
                return string.Format("Equals((object)@{0})", indexOfParameter);
            }

            return q.EndsWith("$") ? makeClause("EndsWith", q.Substring(0, q.Length - 1)) : makeClause("Contains", q);
        }
    }
}