using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Brainary.Commons.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// Obtain <see cref="DisplayAttribute"/> attached to an enum value
        /// </summary>
        /// <param name="value">Enum value</param>
        public static DisplayAttribute? GetDisplayAttribute(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute;
        }

        /// <summary>
        /// Obtain description from <see cref="DisplayAttribute"/> attached to an enum value
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <returns>Description string</returns>
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Description ?? string.Empty;
        }

        /// <summary>
        /// Obtain name from <see cref="DisplayAttribute"/> attached to an enum value
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <returns>Display Name string or enum string value</returns>
        public static string GetDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? value.ToString("G");
        }

        /// <summary>
        /// Obtain short name from <see cref="DisplayAttribute"/> attached to an enum value
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <returns>Short Name string or enum string value</returns>
        public static string GetShortName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute?.ShortName ?? value.ToString("G");
        }

        /// <summary>
        /// Obtain group name from <see cref="DisplayAttribute"/> attached to an enum value
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <returns>Group Name string</returns>
        public static string GetGroupName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute?.GroupName ?? string.Empty;
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