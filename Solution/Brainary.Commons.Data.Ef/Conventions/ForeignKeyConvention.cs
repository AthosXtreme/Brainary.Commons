namespace Brainary.Commons.Data.Conventions
{
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Linq;

    public class ForeignKeyConvention : IStoreModelConvention<AssociationType>
    {
        public void Apply(AssociationType association, DbModel model)
        {
            const string NameTemplate = "{0}Id";
            if (association.IsForeignKey)
            {
                var c = association.Constraint;
                if (c.ToProperties.Count == c.FromProperties.Count && c.ToProperties.Any(a => a.Name.EndsWith("_Id")))
                {
                    foreach (var item in c.ToProperties.Select((s, i) => new { Index = i, Property = s }))
                    {
                        item.Property.Name = string.Format(NameTemplate, c.FromProperties[item.Index].DeclaringType.Name);
                    }
                }
            }
        }
    }
}
