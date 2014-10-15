namespace Brainary.Commons.Data
{
    using System.Collections.Generic;

    /// <summary>
    /// <see cref="IResultQuery{T}"/>"/> implementation
    /// </summary>
    /// <typeparam name="T">List type</typeparam>
    public class ResultQuery<T> : IResultQuery<T>
    {
        public int Count { get; set; }

        public int Page { get; set; }

        public IList<T> Recordset { get; set; }
    }
}