namespace Brainary.Commons.Extensions
{
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

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
            var serializer = new XmlSerializer(typeof(T));
            using (var sw = new StringWriter())
            {
                var writer = XmlWriter.Create(sw);
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
        public static T Deserialize<T>(this string xmlObject) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));

            StringReader sr = null;
            try
            {
                sr = new StringReader(xmlObject);
                using (var xmlReader = new XmlTextReader(sr))
                {
                    sr = null;
                    return (T)serializer.Deserialize(xmlReader);
                }
            }
            finally
            {
                if (sr != null) sr.Dispose();
            }
        }
    }
}
