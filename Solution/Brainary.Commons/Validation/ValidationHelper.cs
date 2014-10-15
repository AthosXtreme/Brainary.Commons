namespace Brainary.Commons.Validation
{
    /// <summary>
    /// Stand alone validation helper
    /// </summary>
    public class ValidationHelper
    {
        /// <summary>
        /// Shorcut to <see cref="EntityValidator{T}"/>
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity instance</param>
        /// <returns>Validation result</returns>
        public static EntityValidationResult ValidateEntity<T>(T entity) where T : class
        {
            return new EntityValidator<T>().Validate(entity);
        }
    }
}
