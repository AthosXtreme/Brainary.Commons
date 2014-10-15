namespace Brainary.Commons.Web.Validation.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class RequiredWhenParentAttribute : Commons.Validation.Attributes.RequiredWhenParentAttribute, IClientValidatable
    {
        public RequiredWhenParentAttribute(string parentPropertyName)
            : base(parentPropertyName)
        {
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "requiredwhenparent"
            };
            rule.ValidationParameters["parentproperty"] = ParentPropertyName;
            yield return rule;
        }
    }
}
