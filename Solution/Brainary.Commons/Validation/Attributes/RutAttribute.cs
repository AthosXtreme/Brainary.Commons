namespace Brainary.Commons.Validation.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text.RegularExpressions;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class RutAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return true;

            var regexpm = new Regex(@"^(([1-9])|([1-9][0-9]))(((\.\d{3}){2}\-)|((\d{3}){2}\-)|((\d{3}){2}))[\dkK]$");

            // Expresion para RUTs bajo el millon
            var regexpc = new Regex(@"^(((\d{3}\.\d{3})\-)|((\d{3}){2}\-)|((\d{3}){2}))[\dkK]$");
            if (regexpm.IsMatch(value.ToString()) || regexpc.IsMatch(value.ToString()))
            {
                var full = value.ToString().Replace(".", string.Empty).Replace("-", string.Empty);
                var rut = full.Substring(0, full.Length - 1);
                var dv = full.Substring(full.Length - 1, 1).ToUpper();
                return DigitoVerificador(int.Parse(rut)) == dv;
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(Messages.InvalidField, name);
        }

        private static string DigitoVerificador(int rut)
        {
            var contador = 2;
            var acumulador = 0;

            while (rut != 0)
            {
                var multiplo = (rut % 10) * contador;
                acumulador = acumulador + multiplo;
                rut = rut / 10;
                contador = contador + 1;
                if (contador == 8)
                {
                    contador = 2;
                }
            }

            var digito = 11 - (acumulador % 11);
            var rutDigito = digito.ToString(CultureInfo.InvariantCulture).Trim();
            if (digito == 10)
            {
                rutDigito = "K";
            }

            if (digito == 11)
            {
                rutDigito = "0";
            }

            return rutDigito;
        }
    }
}
