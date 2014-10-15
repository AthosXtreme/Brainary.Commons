namespace Brainary.Commons.Validation.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AtLeastOneRequiredAttribute : ValidationAttribute
    {
        public AtLeastOneRequiredAttribute(string groupName)
        {
            GroupName = groupName;
        }

        public string GroupName { get; private set; }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(Messages.AtLeastOneRequired, name);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return GetGroupProperties(validationContext.ObjectType).Select(p => string.Format("{0}", p.GetValue(validationContext.ObjectInstance, null))).All(string.IsNullOrWhiteSpace) ? new ValidationResult(FormatErrorMessage(GroupName)) : null;
        }

        protected IEnumerable<PropertyInfo> GetGroupProperties(Type type)
        {
            return
                from property in type.GetProperties()
                let attributes = property.GetCustomAttributes(typeof(AtLeastOneRequiredAttribute), false).OfType<AtLeastOneRequiredAttribute>()
                where attributes.Any()
                from attribute in attributes
                where attribute.GroupName == GroupName
                select property;
        }
    }
}
