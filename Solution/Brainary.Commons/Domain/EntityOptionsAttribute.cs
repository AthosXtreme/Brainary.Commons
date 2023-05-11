namespace Brainary.Commons.Domain
{
    /// <summary>
    /// Options for <see cref="Entity"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EntityOptionsAttribute : Attribute
    {
        /// <summary>
        /// Prevent identity for Id field (applies to short, int, long and Guid)
        /// </summary>
        public bool PreventIdentityId { get; set; } = false;

        /// <summary>
        /// Set maximum length for Id field (applies to string and array)
        /// </summary>
        public int MaxLengthId { get; set; } = 16;
    }
}
