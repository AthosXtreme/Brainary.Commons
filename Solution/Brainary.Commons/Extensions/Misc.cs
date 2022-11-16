
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
    }
}