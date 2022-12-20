namespace Brainary.Commons.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// Determines if an element exists in a sequence
        /// </summary>
        public static bool In<T>(this T value, params T[] items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            return items.Contains(value);
        }

        /// <summary>
        /// Returns custom True/False/Null strings
        /// </summary>
        /// <param name="value">Bool value</param>
        /// <param name="trueString">True string</param>
        /// <param name="falseString">False string</param>
        /// <param name="nullString">Null string</param>
        /// <returns>Deined custom string for each case</returns>
        public static string ToString(this bool? value, string trueString, string falseString, string nullString = "Undefined")
        {
            return value == null ? nullString : value.Value ? trueString : falseString;
        }

        /// <summary>
        /// Returns custom True/False strings
        /// </summary>
        /// <param name="value">Bool value</param>
        /// <param name="trueString">True string</param>
        /// <param name="falseString">False string</param>
        /// <returns>Deined custom string for each case</returns>
        public static string ToString(this bool value, string trueString, string falseString)
        {
#pragma warning disable CS8625
            return ToString(value, trueString, falseString, null);
#pragma warning restore CS8625
        }
    }
}