namespace Brainary.Commons.Extensions
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;

    public static partial class Extensions
    {
        /// <summary>
        /// Generates a semicolon delimited csv string
        /// </summary>
        /// <typeparam name="T">List type</typeparam>
        /// <param name="list">Object list</param>
        /// <returns>Plain text csv</returns>
        public static string ToCsv<T>(this IList<T> list) where T : class
        {
            return GenerateFile(list, ';').ToString();
        }

        /// <summary>
        /// Generates a character delimited csv string
        /// </summary>
        /// <typeparam name="T">List type</typeparam>
        /// <param name="list">Object list</param>
        /// <param name="separator">Separator character</param>
        /// <returns>Plain text csv</returns>
        public static string ToCsv<T>(this IList<T> list, char separator) where T : class
        {
            return GenerateFile(list, separator).ToString();
        }

        /// <summary>
        /// Generates a character delimited csv string
        /// </summary>
        /// <param name="table">Data source</param>
        /// <returns>Plain text csv</returns>
        public static string ToCsv(this DataTable table)
        {
            return GenerateFile(table, ';').ToString();
        }

        /// <summary>
        /// Generates a character delimited csv string
        /// </summary>
        /// <param name="table">Data source</param>
        /// <param name="separator">Separator character</param>
        /// <returns>Plain text csv</returns>
        public static string ToCsv(this DataTable table, char separator)
        {
            return GenerateFile(table, separator).ToString();
        }

        private static StringBuilder GenerateFile<T>(this IList<T> list, char separator)
        {
            var sb = new StringBuilder();
            if (list.Any())
            {
                sb.Append(HeadersBuilder(typeof(T).GetProperties().Select(s => s.Name), separator));
                sb.Append(ContentBuilder(list, separator));
            }

            return sb;
        }

        private static StringBuilder GenerateFile(this DataTable table, char separator)
        {
            var sb = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                sb.Append(HeadersBuilder(table.Columns.Cast<DataColumn>().Select(s => s.ColumnName), separator));
                sb.Append(ContentBuilder(table, separator));
            }

            return sb;
        }

        private static StringBuilder HeadersBuilder(IEnumerable<string> names, char separator)
        {
            var textOutput = new StringBuilder();
            var stringFile = names.Aggregate((current, val) => string.Format("{0}{1}{2}", current, separator, val));
            textOutput.AppendLine(stringFile);
            return textOutput;
        }

        private static StringBuilder ContentBuilder<T>(IEnumerable<T> data, char separator)
        {
            var textOutput = new StringBuilder();

            var props = typeof(T).GetProperties();
            foreach (var stringFile in data.Select(i => props.Select(s => s.GetValue(i, null)).Aggregate((current, val) => string.Format("{0}{1}{2}", current, separator, val))))
                textOutput.AppendLine(stringFile.ToString());

            return textOutput;
        }

        private static StringBuilder ContentBuilder(DataTable data, char separator)
        {
            var textOutput = new StringBuilder();

            var stringFile = data.Rows.Cast<DataRow>().Select(s => s.ItemArray.Aggregate((current, val) => string.Format("{0}{1}{2}", current, separator, val)));
            foreach (var o in stringFile)
                textOutput.AppendLine(o.ToString());

            return textOutput;
        }
    }
}