namespace Brainary.Commons.Validation
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Validation summary
    /// </summary>
    public class EntityValidationResult
    {
        public EntityValidationResult()
            : this(null)
        {
        }

        /// <summary>
        /// Cosntructor
        /// </summary>
        /// <param name="errors">Validation errors</param>
        public EntityValidationResult(IList<ValidationResult> errors)
        {
            Errors = errors ?? new List<ValidationResult>();
        }

        /// <summary>
        /// Gets validation errors
        /// </summary>
        public IList<ValidationResult> Errors { get; private set; }

        /// <summary>
        /// Determine if result is valid
        /// </summary>
        public bool IsValid
        {
            get { return Errors.Count == 0; }
        }
    }
}