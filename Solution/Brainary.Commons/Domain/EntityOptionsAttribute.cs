namespace Brainary.Commons.Domain
{
    /// <summary>
    /// Options for <see cref="Entity"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EntityOptionsAttribute : Attribute
    {
        public bool IdentityId { get; set; } = true;
    }
}
