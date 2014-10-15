namespace Brainary.Commons.Data
{
    using System.Collections.Generic;

    /// <summary>
    /// Basic database command execution return class
    /// </summary>
    public class ExecuteResult
    {
        /// <summary>
        /// Output parameters
        /// </summary>
        public IDictionary<string, object> OutParams { get; set; }

        /// <summary>
        /// Return value
        /// </summary>
        public object ReturnValue { get; set; }
    }
}