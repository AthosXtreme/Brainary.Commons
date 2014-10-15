namespace Brainary.Commons.Data
{
    /// <summary>
    /// Database command execution interface
    /// </summary>
    public interface IDataAccess
    {
        /// <summary>
        /// Creates a command
        /// </summary>
        /// <param name="spName">Database program name</param>
        /// <returns>Created command</returns>
        SpCommand CreateSpCommand(string spName);

        /// <summary>
        /// Executes a command with output parameters (if any) and table results
        /// </summary>
        /// <param name="spCommand">Command to execute</param>
        /// <returns>Execution result</returns>
        ExecuteQueryResult ExecuteQuery(SpCommand spCommand);

        /// <summary>
        /// Executes a command with output parameters (if any) and typed scalar results
        /// </summary>
        /// <param name="spCommand">Command to execute</param>
        /// <returns>Execution result</returns>
        ExecuteScalarResult<T> ExecuteScalar<T>(SpCommand spCommand);

        /// <summary>
        /// Executes a command with output parameters (if any) results
        /// </summary>
        /// <param name="spCommand">Command to execute</param>
        /// <returns>Execution result</returns>
        ExecuteResult ExecuteNonQuery(SpCommand spCommand);
    }
}
