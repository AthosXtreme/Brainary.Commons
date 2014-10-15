namespace Brainary.Commons.Web.Validation.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class DateLessThanAttribute : Commons.Validation.Attributes.DateLessThanAttribute, IClientValidatable
    {
        public DateLessThanAttribute()
        {
        }

        public DateLessThanAttribute(string propertyName)
            : base(propertyName)
        {
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "datelessthan"
            };
            rule.ValidationParameters["property"] = PropertyName;
            rule.ValidationParameters["allowequal"] = AllowEqual;

            yield return rule;
        }
    }
}
