namespace Brainary.Commons.Validation.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequiredWhenParentAttribute : ValidationAttribute
    {
        public RequiredWhenParentAttribute(string parentPropertyName)
        {
            ParentPropertyName = parentPropertyName;
        }
        
        public string ParentPropertyName { get; private set; }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(Messages.RequiredField, name);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var parentPropertyInfo = validationContext.ObjectType.GetProperty(ParentPropertyName);
            if (string.IsNullOrEmpty(string.Format("{0}", parentPropertyInfo.GetValue(validationContext.ObjectInstance, null)))) return null;
            if (!string.IsNullOrEmpty(string.Format("{0}", value))) return null;
            var message = FormatErrorMessage(validationContext.DisplayName);
            return new ValidationResult(message);
        }
    }
}
