namespace Brainary.Commons.Extensions
{
    using System;

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
    }
}