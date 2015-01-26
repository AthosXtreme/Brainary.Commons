namespace Brainary.Commons.Validation.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequiredWhenParentAttribute : ValidationAttribute
    {
        public RequiredWhenParentAttribute(string parentPropertyName, object parentPropertyValue)
        {
            ParentPropertyName = parentPropertyName;
            ParentPropertyValue = parentPropertyValue;
        }

        public RequiredWhenParentAttribute(string parentPropertyName)
            : this(parentPropertyName, null)
        {
        }

        public string ParentPropertyName { get; private set; }

        public object ParentPropertyValue { get; private set; }

        public override bool RequiresValidationContext
        {
            get
            {
                return true;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(Messages.RequiredField, name);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var parentPropertyInfo = validationContext.ObjectType.GetProperty(ParentPropertyName);
            var parentPropertyValue = parentPropertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (ParentPropertyValue == null)
            {
                if (string.IsNullOrEmpty(string.Format("{0}", parentPropertyValue))) return null;
            }
            else
            {
                if (!ParentPropertyValue.Equals(parentPropertyValue)) return null;
            }

            if (!string.IsNullOrEmpty(string.Format("{0}", value))) return null;
            var message = FormatErrorMessage(validationContext.DisplayName);
            return new ValidationResult(message);
        }
    }
}
