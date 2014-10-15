namespace Brainary.Commons.Data.Conventions
{
    using System.Data.Entity.ModelConfiguration.Conventions;

    public class PrimaryKeyConvention : Convention
    {
        public PrimaryKeyConvention()
        {
            const string NameTemplate = "{0}Id";
            Properties().Where(w => w.Name == "Id")
                .Configure(p => p.IsKey().HasColumnName(string.Format(NameTemplate, p.ClrPropertyInfo.ReflectedType.Name)));
        }
    }
}
