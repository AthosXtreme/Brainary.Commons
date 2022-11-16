using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Brainary.Commons.Extensions
{
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
            var attribute = field != null ? Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute : null;
            return attribute == null ? value.ToString("G") : attribute.Description;
        }

        /// <summary>
        /// Obtain display name attribute from enum value
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <returns>Description string</returns>
        public static string GetDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field != null ? Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) as DisplayAttribute : null;
            return attribute == null ? value.ToString("G") : attribute?.Name ?? string.Empty;
        }

        /// <summary>
        /// Converts a string into an enum of a given type
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="value">String value</param>
        /// <returns>Enum value</returns>
        public static T AsEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// Obtain current flag values list
        /// </summary>
        /// <param name="flags">Enum flags</param>
        /// <returns>Listed flags</returns>
        public static IEnumerable<Enum> GetUniqueFlags(this Enum flags)
        {
            ulong flag = 1;
            foreach (var value in Enum.GetValues(flags.GetType()).Cast<Enum>())
            {
                var bits = Convert.ToUInt64(value);
                while (flag < bits)
                {
                    flag <<= 1;
                }

                if (flag == bits && flags.HasFlag(value))
                {
                    yield return value;
                }
            }
        }
    }
}