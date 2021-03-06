﻿namespace Brainary.Commons.Validation.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DateLessThanAttribute : ValidationAttribute
    {
        public DateLessThanAttribute()
        {
        }

        public DateLessThanAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        public bool AllowEqual { get; set; }

        public string PropertyName { get; private set; }

        public override bool RequiresValidationContext
        {
            get
            {
                return true;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(AllowEqual ? Messages.MustBeLessOrEqual : Messages.MustBeLessThan, name, !string.IsNullOrEmpty(PropertyName) ? PropertyName : Messages.TheActualDate);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return null;

            var startDate = DateTime.Now.Date;
            var thisDate = ((DateTime)value).Date;

            if (!string.IsNullOrEmpty(PropertyName))
            {
                var basePropertyInfo = validationContext.ObjectType.GetProperty(PropertyName);
                startDate = ((DateTime)basePropertyInfo.GetValue(validationContext.ObjectInstance, null)).Date;
            }

            if (AllowEqual && thisDate <= startDate) return null;
            if (!AllowEqual && thisDate < startDate) return null;

            var message = FormatErrorMessage(validationContext.DisplayName);
            return new ValidationResult(message);
        }
    }
}
