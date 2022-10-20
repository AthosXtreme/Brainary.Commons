namespace Brainary.Commons.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ComputedColumnSqlAttribute : Attribute
    {
        public string Statement { get; set; }

        public ComputedColumnSqlAttribute(string statement)
        {
            if (string.IsNullOrWhiteSpace(statement))
                throw new ArgumentNullException(nameof(statement));
            Statement = statement;
        }
    }
}