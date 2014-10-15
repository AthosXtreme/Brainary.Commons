namespace Brainary.Commons.Data
{
    using System.Data;

    /// <summary>
    /// Database command execution with results return class
    /// </summary>
    public class ExecuteQueryResult : ExecuteResult
    {
        /// <summary>
        /// Results table
        /// </summary>
        public DataTable Table { get; set; }
    }
}