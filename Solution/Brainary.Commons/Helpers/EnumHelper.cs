namespace Brainary.Commons.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    using Brainary.Commons.Extensions;

    /// <summary>
    /// Enum helper methods
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// Get <see cref="NameValueCollection"/> from enum type with number name format
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns><see cref="NameValueCollection"/></returns>
        public static NameValueCollection ToCollection<T>() where T : struct
        {
            return ToCollection<T>("D");
        }

        /// <summary>
        /// Get <see cref="NameValueCollection"/> from enum type with specified name format
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns><see cref="NameValueCollection"/></returns>
        public static NameValueCollection ToCollection<T>(string nameFormat) where T : struct
        {
            var enumType = typeof(T);
            if (!enumType.IsEnum)
                throw new InvalidOperationException("Type must be enum");

            var collection = new NameValueCollection();
            foreach (var @enum in Enum.GetValues(enumType).Cast<Enum>())
            {
                collection.Add(@enum.ToString(nameFormat), @enum.GetDescription());
            }

            return collection;
        }

        /// <summary>
        /// Get dictionary from enum type with number key format
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>Collection</returns>
        public static IDictionary<string, string> ToDictionary<T>() where T : struct
        {
            return ToDictionary<T>("D");
        }

        /// <summary>
        /// Get dictionary from enum type with specified key format
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>Collection</returns>
        public static IDictionary<string, string> ToDictionary<T>(string keyFormat) where T : struct
        {
            var enumType = typeof(T);
            if (!enumType.IsEnum)
                throw new InvalidOperationException("Type must be enum");

            return Enum.GetValues(enumType).Cast<Enum>().ToDictionary(k => k.ToString(keyFormat), v => v.GetDescription());
        }
    }
}