namespace Brainary.Commons.Data
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for typed paged result list
    /// </summary>
    /// <typeparam name="T">List type</typeparam>
    public interface IResultQuery<T>
    {
        int Count { get; set; }

        int Page { get; set; }

        IList<T> Recordset { get; set; }
    }
}