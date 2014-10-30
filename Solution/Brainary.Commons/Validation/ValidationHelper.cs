namespace Brainary.Commons.Validation
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Standalone validation helper
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

        /// <summary>
        /// Throws detailed <see cref="ValidationException"/> if ValidateEntity is not valid
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity instance</param>
        public static void AssertValidation<T>(T entity) where T : class
        {
            var result = ValidateEntity(entity);
            if (!result.IsValid)
                throw new ValidationException(result.Errors.Aggregate(new StringBuilder(), (ag, n) => ag.AppendLine(n.ErrorMessage)).ToString());
        }
    }
}
