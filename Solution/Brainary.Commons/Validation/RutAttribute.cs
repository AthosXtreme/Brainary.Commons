using System.ComponentModel.DataAnnotations;

namespace Brainary.Commons.Validation
{
    /// <summary>
    /// Atributo para validación de RUT chileno
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class RutAttribute : DataTypeAttribute
    {
        public RutAttribute() : base("RUT")
        {
            ErrorMessage = "RUT inválido";
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
                return true;

            if (value is not string valueAsString)
                return false;

            return Util.Rut.IsValid(valueAsString);
        }
    }
}
