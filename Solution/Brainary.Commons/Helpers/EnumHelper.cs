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
        /// Get <see cref="NameValueCollection"/> from enum type with number name format
        /// </summary>
        /// <param name="enumType">Enum type</param>
        /// <returns><see cref="NameValueCollection"/></returns>
        public static NameValueCollection ToCollection(Type enumType)
        {
            return ToCollection(enumType, "D");
        }

        /// <summary>
        /// Get <see cref="NameValueCollection"/> from enum type with specified name format
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="nameFormat">Enum value format</param>
        /// <returns><see cref="NameValueCollection"/></returns>
        public static NameValueCollection ToCollection<T>(string nameFormat) where T : struct
        {
            return ToCollection(typeof(T), nameFormat);
        }

        /// <summary>
        /// Get <see cref="NameValueCollection"/> from enum type with specified name format
        /// </summary>
        /// <param name="enumType">Enum type</param>
        /// <param name="nameFormat">Name format</param>
        /// <returns><see cref="NameValueCollection"/></returns>
        public static NameValueCollection ToCollection(Type enumType, string nameFormat)
        {
            if (!enumType.IsEnum)
                throw new InvalidOperationException("Type must be enum");

            var collection = new NameValueCollection();
            foreach (var @enum in Enum.GetValues(enumType).Cast<Enum>())
            {
                collection.Add(@enum.ToString(nameFormat), @enum.GetDisplayName());
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
        /// Get dictionary from enum type with number key format
        /// </summary>
        /// <param name="enumType">Enum type</param>
        /// <returns>Collection</returns>
        public static IDictionary<string, string> ToDictionary(Type enumType)
        {
            return ToDictionary(enumType, "D");
        }

        /// <summary>
        /// Get dictionary from enum type with specified key format
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="keyFormat">Key format</param>
        /// <returns>Collection</returns>
        public static IDictionary<string, string> ToDictionary<T>(string keyFormat) where T : struct
        {
            return ToDictionary(typeof(T), keyFormat);
        }

        /// <summary>
        /// Get dictionary from enum type with specified key format
        /// </summary>
        /// <param name="enumType">Enum type</param>
        /// <param name="keyFormat">Key format</param>
        /// <returns>Collection</returns>
        public static IDictionary<string, string> ToDictionary(Type enumType, string keyFormat)
        {
            if (!enumType.IsEnum)
                throw new InvalidOperationException("Type must be enum");

            return Enum.GetValues(enumType)
                .Cast<Enum>()
                .ToDictionary(k => k.ToString(keyFormat), v => v.GetDisplayName());
        }
    }
}