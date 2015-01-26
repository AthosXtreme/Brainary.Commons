namespace Brainary.Commons.Web.Mvc.Validation.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class DigitsAttribute : Commons.Validation.Attributes.DigitsAttribute, IClientValidatable
    {
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "digits"
            };
        }
    }
}
