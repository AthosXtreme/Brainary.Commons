namespace Brainary.Commons.Data
{
    /// <summary>
    /// Typed scalar database value return class
    /// </summary>
    /// <typeparam name="T">Expected return type</typeparam>
    public class ExecuteScalarResult<T> : ExecuteResult
    {
        /// <summary>
        /// Return value
        /// </summary>
        public T Value { get; set; }
    }
}