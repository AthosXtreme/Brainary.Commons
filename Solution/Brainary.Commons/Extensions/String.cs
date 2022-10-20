using System.Security.Cryptography;
using System.Text;

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
        /// Get MD5 hash from string
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>String</returns>
        public static string GetMd5Hash(this string str)
        {
            using (var md5Hash = MD5.Create())
            {
                var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(str));
                var builder = new StringBuilder();

                foreach (var t in data)
                    builder.Append(t.ToString("x2"));

                return builder.ToString();
            }
        }

        public static string CleanRut(this string str)
        {
            return new string(str.Where(char.IsLetterOrDigit).ToArray());
        }

        public static string FormatRut(this string str)
        {
            var value = str.CleanRut();
            return string.Format("{0}-{1}", int.Parse(value[..^1]).ToString("N0"), value.Last());
        }
    }
}