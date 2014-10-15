namespace Brainary.Commons.Web.Validation.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class DateGreaterThanAttribute : Commons.Validation.Attributes.DateGreaterThanAttribute, IClientValidatable
    {
        public DateGreaterThanAttribute(string propertyName = null)
            : base(propertyName)
        {
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "dategreaterthan"
            };
            rule.ValidationParameters["property"] = PropertyName;
            rule.ValidationParameters["allowequal"] = AllowEqual;

            yield return rule;
        }
    }
}
