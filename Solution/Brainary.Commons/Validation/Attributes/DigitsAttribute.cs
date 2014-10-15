namespace Brainary.Commons.Validation.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DigitsAttribute : DataTypeAttribute
    {
        private static readonly Regex Regex = new Regex(@"^\d*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public DigitsAttribute()
            : base("digits")
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(Messages.MustContainOnlyDigits, name);
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true;
            var valueAsString = value as string;
            return valueAsString != null && Regex.Match(valueAsString).Length > 0;
        }
    }
}
