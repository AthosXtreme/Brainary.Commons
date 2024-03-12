namespace Brainary.Commons.Data.Annotations
{
    /// <summary>
    /// Specifies an entity index
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class IndexAttribute : Attribute
    {
        public IndexAttribute(string propertyName, params string[] additionalPropertyNames)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.");

            if (additionalPropertyNames.Any(string.IsNullOrEmpty))
                throw new ArgumentException($"An element in {nameof(additionalPropertyNames)} is null or empty.");

            PropertyNames = new List<string> { propertyName };
            ((List<string>)PropertyNames).AddRange(additionalPropertyNames);
        }

        public IReadOnlyList<string> PropertyNames { get; }

        public bool IsUnique { get; set; } = false;
    }
}
