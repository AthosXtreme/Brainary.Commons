namespace Brainary.Commons.Validation.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RelativeDateRangeAttribute : RangeAttribute
    {
        public RelativeDateRangeAttribute(int fromDays, int toDays)
            : base(typeof(DateTime), DateTime.Now.AddDays(fromDays).ToShortDateString(), DateTime.Now.AddDays(toDays).ToShortDateString())
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, ((DateTime)Minimum).ToShortDateString(), ((DateTime)Maximum).ToShortDateString());
        }
    }
}