namespace Brainary.Commons.Data
{
    using System.Data;
    using System.Data.Common;
    using System.Linq;

    using Microsoft.Practices.EnterpriseLibrary.Data;

    /// <summary>
    /// SQL Server implementation of <see cref="IDataAccess"/>
    /// </summary>
    public class MsSqlDataAccess : IDataAccess
    {
        private readonly Database database;

        public MsSqlDataAccess()
        {
            database = DatabaseFactory.CreateDatabase();
        }

        public MsSqlDataAccess(string name)
        {
            database = DatabaseFactory.CreateDatabase(name);
        }

        public MsSqlDataAccess(Database database)
        {
            this.database = database;
        }

        public SpCommand CreateSpCommand(string spName)
        {
            return new MsSqlSpCommand(spName);
        }

        public ExecuteQueryResult ExecuteQuery(SpCommand spCommand)
        {
            var command = database.GetStoredProcCommand(spCommand.SpName);
            command.CommandTimeout = spCommand.Timeout;
            command.Parameters.AddRange(spCommand.GetParameters());
            using (var dr = database.ExecuteReader(command))
            {
                using (var dt = new DataTable())
                {
                    dt.Load(dr);
                    var response = new ExecuteQueryResult
                    {
                        OutParams = command.Parameters.Cast<DbParameter>().Where(w => w.Direction == ParameterDirection.Output).ToDictionary(k => k.ParameterName, v => v.Value),
                        ReturnValue = command.Parameters.Cast<DbParameter>().First(w => w.Direction == ParameterDirection.ReturnValue).Value,
                        Table = dt
                    };

                    return response;
                }
            }
        }

        public ExecuteScalarResult<T> ExecuteScalar<T>(SpCommand spCommand)
        {
            var command = database.GetStoredProcCommand(spCommand.SpName);
            command.CommandTimeout = spCommand.Timeout;
            command.Parameters.AddRange(spCommand.GetParameters());
            var sc = database.ExecuteScalar(command);

            var response = new ExecuteScalarResult<T>
            {
                OutParams = command.Parameters.Cast<DbParameter>().Where(w => w.Direction == ParameterDirection.Output).ToDictionary(k => k.ParameterName, v => v.Value),
                ReturnValue = command.Parameters.Cast<DbParameter>().First(w => w.Direction == ParameterDirection.ReturnValue).Value,
                Value = (T)sc
            };
            return response;
        }

        public ExecuteResult ExecuteNonQuery(SpCommand spCommand)
        {
            var command = database.GetStoredProcCommand(spCommand.SpName);
            command.CommandTimeout = spCommand.Timeout;
            command.Parameters.AddRange(spCommand.GetParameters());
            database.ExecuteReader(command);
            var response = new ExecuteResult
            {
                OutParams = command.Parameters.Cast<DbParameter>().Where(w => w.Direction == ParameterDirection.Output).ToDictionary(k => k.ParameterName, v => v.Value),
                ReturnValue = command.Parameters.Cast<DbParameter>().First(w => w.Direction == ParameterDirection.ReturnValue).Value
            };
            return response;
        }
    }
}
