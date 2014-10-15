namespace Brainary.Commons.Extensions
{
    using System;
    using System.ComponentModel;

    public static partial class Extensions
    {
        /// <summary>
        /// Obtain description attribute from enum value
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <returns>Description string</returns>
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute == null ? value.ToString("G") : attribute.Description;
        }

        /// <summary>
        /// Obtain enum value from string
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="value">String value</param>
        /// <returns>Enum value</returns>
        public static T ParseEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }
    }
}