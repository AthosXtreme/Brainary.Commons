namespace Brainary.Commons.Validation
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Stand alone validation
    /// </summary>
    /// <typeparam name="T">Validation type</typeparam>
    public class EntityValidator<T> where T : class
    {
        /// <summary>
        /// Validate an entity
        /// </summary>
        /// <param name="entity">Object</param>
        /// <returns>Validation result</returns>
        public EntityValidationResult Validate(T entity)
        {
            var validationResults = new List<ValidationResult>();

            if (entity == null)
                return new EntityValidationResult(new List<ValidationResult> { new ValidationResult(Messages.MissingObjectInstance) });

            var vc = new ValidationContext(entity, null, null);
            Validator.TryValidateObject(entity, vc, validationResults, true);
            return new EntityValidationResult(validationResults);
        }
    }
}