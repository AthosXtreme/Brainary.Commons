namespace Brainary.Commons.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// Get byte array from string
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>Byte array</returns>
        public static byte[] GetBytes(this string str)
        {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// Replaces {key} with "value" in a string
        /// </summary>
        /// <param name="format">Composite format string</param>
        /// <param name="args">Anonymous object with key/values to apply over <paramref name="format"/></param>
        public static string Format(this string format, object args)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            if (!args.GetType().IsAnonymousType())
                throw new FormatException($"{nameof(args)} Type is invalid.");

            var parameters = args.GetType().GetProperties().ToDictionary(x => $"{{{x.Name}}}", x => x.GetValue(args, null));

            var sb = new System.Text.StringBuilder(format);
            foreach (var kv in parameters)
                sb.Replace(kv.Key, kv.Value != null ? kv.Value.ToString() : "");

            return sb.ToString();
        }
    }
}