namespace Brainary.Commons.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DefaultValueSqlAttribute : Attribute
    {
        public string Statement { get; set; }

        public DefaultValueSqlAttribute(string statement)
        {
            if (string.IsNullOrWhiteSpace(statement))
                throw new ArgumentNullException(nameof(statement));
            Statement = statement;
        }
    }
}