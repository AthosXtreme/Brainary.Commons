using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;

namespace Brainary.Commons.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// Serialize an object to json
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="instance">Object instance</param>
        /// <returns>Json string</returns>
        public static string JsonSerialize<T>(this T instance) where T : class
        {
            return JsonSerialize(instance, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, WriteIndented = true });
        }

        /// <summary>
        /// Serialize an object to json
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="instance">Object instance</param>
        /// <param name="options">Serialization options</param>
        /// <returns>Json string</returns>
        public static string JsonSerialize<T>(this T instance, JsonSerializerOptions? options) where T : class
        {
            using var stream = new MemoryStream();
            JsonSerializer.Serialize(stream, instance, options);
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>
        /// Deserialize an json string to object
        /// </summary>
        /// <typeparam name="T">Expected object type</typeparam>
        /// <param name="json">Json string</param>
        /// <returns>Object</returns>
        public static T? JsonDeserialize<T>(this string json) where T : class
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var message = JsonSerializer.Deserialize<T>(stream, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = ReferenceHandler.Preserve
            });

            return message;
        }
    }
}