namespace Brainary.Commons.Data
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;

    /// <summary>
    /// SQL Server implementation of <see cref="SpCommand"/>
    /// </summary>
    public class MsSqlSpCommand : SpCommand
    {
        public MsSqlSpCommand(string spName)
            : base(spName)
        {
        }

        protected override DbParameter CreateInParameter(string name, DbType dbType, object value)
        {
            return new SqlParameter { ParameterName = name, DbType = dbType, Direction = ParameterDirection.Input, Value = value ?? DBNull.Value };
        }

        protected override DbParameter CreateOutParameter(string name, DbType dbType, int size)
        {
            return new SqlParameter { ParameterName = name, DbType = dbType, Direction = ParameterDirection.Output, Size = size };
        }

        protected override DbParameter CreateReturnValueParameter(DbType dbType, int size)
        {
            return new SqlParameter { ParameterName = "ReturnValue", DbType = dbType, Direction = ParameterDirection.ReturnValue, Size = size };
        }
    }
}