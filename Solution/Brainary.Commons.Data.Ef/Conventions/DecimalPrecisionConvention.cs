namespace Brainary.Commons.Data.Conventions
{
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Linq;

    using Brainary.Commons.Annotations;

    public class DecimalPrecisionConvention : Convention
    {
        public DecimalPrecisionConvention()
        {
            Properties<decimal>().Configure(p =>
                {
                    var m = p.ClrPropertyInfo.GetCustomAttributes(false).OfType<DecimalPrecisionAttribute>().FirstOrDefault() ?? new DecimalPrecisionAttribute(14, 4);
                    p.HasPrecision(m.Precision, m.Scale);
                });
        }
    }
}
