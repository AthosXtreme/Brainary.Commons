namespace Brainary.Commons.Util
{
    public static class Misc
    {
        /// <summary>
        /// Generates random filename (without extension)
        /// </summary>
        /// <param name="pre">Optional prefix</param>
        /// <returns>FileName string</returns>
        public static string CreateFileName(string? pre = null)
        {
            pre = !string.IsNullOrWhiteSpace(pre) ? $"{pre}_" : string.Empty;
            var hash = Math.Abs(Guid.NewGuid().GetHashCode());
            var ends = Path.GetRandomFileName().Split('.');
            return $"{pre}{ends[0]}_{hash}_{ends[1]}";
        }
    }
}
