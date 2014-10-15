namespace Brainary.Commons.Web.Validation.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class AtLeastOneRequiredAttribute : Commons.Validation.Attributes.AtLeastOneRequiredAttribute, IClientValidatable
    {
        public AtLeastOneRequiredAttribute(string groupName)
            : base(groupName)
        {
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var groupProperties = GetGroupProperties(metadata.ContainerType).Select(p => p.Name);
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(GroupName),
                ValidationType = "atleastonerequired"
            };
            rule.ValidationParameters["propertynames"] = string.Join(",", groupProperties);
            yield return rule;
        }
    }
}
