namespace Brainary.Commons.Data
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;

    /// <summary>
    /// Base class for database programs execution
    /// </summary>
    public abstract class SpCommand
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spName">Database program name</param>
        protected SpCommand(string spName)
        {
            SpName = spName;
            Timeout = 30;
            ReturnValueType = DbType.Int32;
            InParameters = new List<DbParameter>();
            OutParameters = new List<DbParameter>();
        }

        /// <summary>
        /// Database program name
        /// </summary>
        public string SpName { get; private set; }

        /// <summary>
        /// Command timeout
        /// </summary>
        public int Timeout { get; set; }

        private DbType ReturnValueType { get; set; }

        private IList<DbParameter> InParameters { get; set; }

        private IList<DbParameter> OutParameters { get; set; }

        /// <summary>
        /// Add input parameter
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="dbType">DB parameter type</param>
        /// <param name="value">Parameter value</param>
        public void AddInParameter(string name, DbType dbType, object value)
        {
            InParameters.Add(CreateInParameter(name, dbType, value));
        }

        /// <summary>
        /// Add output parameter
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="dbType">DB parameter type</param>
        public void AddOutParameter(string name, DbType dbType)
        {
            InParameters.Add(CreateOutParameter(name, dbType, 1024));
        }

        /// <summary>
        /// Add output parameter
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="dbType">DB parameter type</param>
        /// <param name="size">Parameter size in bytes</param>
        public void AddOutParameter(string name, DbType dbType, int size)
        {
            InParameters.Add(CreateOutParameter(name, dbType, size));
        }

        /// <summary>
        /// Return current parameters
        /// </summary>
        /// <returns>Parameters array</returns>
        public DbParameter[] GetParameters()
        {
            return InParameters.Concat(OutParameters).Concat(new[] { CreateReturnValueParameter(ReturnValueType, 1024) }).ToArray();
        }

        /// <summary>
        /// Rerurn a new input parameter
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="dbType">Parameter type</param>
        /// <param name="value">Parameter value</param>
        /// <returns>New DbParameter</returns>
        protected abstract DbParameter CreateInParameter(string name, DbType dbType, object value);

        /// <summary>
        /// Rerurn a new output parameter
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="dbType">Parameter type</param>
        /// <param name="size">Parameter size in bytes</param>
        /// <returns>New DbParameter</returns>
        protected abstract DbParameter CreateOutParameter(string name, DbType dbType, int size);

        /// <summary>
        /// Specifies the return value parameter
        /// </summary>
        /// <param name="dbType">Parameter type</param>
        /// <param name="size">Parameter size in bytes</param>
        /// <returns>New return value DbParameter</returns>
        protected abstract DbParameter CreateReturnValueParameter(DbType dbType, int size);
    }
}