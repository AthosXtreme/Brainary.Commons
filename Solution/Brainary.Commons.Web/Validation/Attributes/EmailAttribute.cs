namespace Brainary.Commons.Web.Validation.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class EmailAttribute : Commons.Validation.Attributes.EmailAttribute, IClientValidatable
    {
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "email"
            };
        }
    }
}
