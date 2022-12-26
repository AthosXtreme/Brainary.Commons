using System.Xml;
using System.Xml.Serialization;

namespace Brainary.Commons.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// Serialize an object to xml
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="instance">Object instance</param>
        /// <returns>Xml string</returns>
        public static string Serialize<T>(this T instance) where T : class
        {
            return Serialize(instance, null);
        }

        /// <summary>
        /// Serialize an object to xml
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="instance">Object instance</param>
        /// <param name="settings">Prevent XML declaration tag on first line</param>
        /// <returns>Xml string</returns>
        public static string Serialize<T>(this T instance, XmlWriterSettings? settings) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var sw = new StringWriter())
            {
                var writer = XmlWriter.Create(sw, settings);
                serializer.Serialize(writer, instance);
                return sw.ToString();
            }
        }

        /// <summary>
        /// Deserialize an xml string to object
        /// </summary>
        /// <typeparam name="T">Expected object type</typeparam>
        /// <param name="xmlObject">Xml string</param>
        /// <returns>Object</returns>
        public static T? Deserialize<T>(this string xmlObject) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));

            StringReader? sr = null;
            try
            {
                sr = new StringReader(xmlObject);
                using var xmlReader = new XmlTextReader(sr);
                sr = null;
                return (T?)serializer.Deserialize(xmlReader);
            }
            finally
            {
                sr?.Dispose();
            }
        }
    }
}