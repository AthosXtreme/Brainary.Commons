namespace Brainary.Commons.Data
{
    using System;
    using System.Data;
    using System.Data.Common;

    using Sybase.Data.AseClient;

    /// <summary>
    /// Sybase implementation of <see cref="SpCommand"/>
    /// </summary>
    public class SybaseSpCommand : SpCommand
    {
        public SybaseSpCommand(string spName)
            : base(spName)
        {
        }

        protected override DbParameter CreateInParameter(string name, DbType dbType, object value)
        {
            return new AseParameter { ParameterName = name, DbType = dbType, Direction = ParameterDirection.Input, Value = value ?? DBNull.Value };
        }

        protected override DbParameter CreateOutParameter(string name, DbType dbType, int size)
        {
            return new AseParameter { ParameterName = name, DbType = dbType, Direction = ParameterDirection.Output, Size = size };
        }

        protected override DbParameter CreateReturnValueParameter(DbType dbType, int size)
        {
            return new AseParameter { ParameterName = "ReturnValue", DbType = dbType, Direction = ParameterDirection.ReturnValue, Size = size };
        }
    }
}