using System.Globalization;

namespace Brainary.Commons.Util
{
    /// <summary>
    /// Utilidad para representación y operaciones de RUT chileno
    /// </summary>
    public readonly struct Rut
    {
        private const int inicioEmpresa = 48000000; //SII asigna desde esta cantidad
        private readonly CultureInfo culture = CultureInfo.CreateSpecificCulture("es-CL");

        public string Num { get;  init; }
        public string Dv { get; init; }

        public Rut(string rut)
        {
            Num = rut.Substring(0, rut.Length - 1).TrimEnd('-');
            Dv = rut.Substring(rut.Length - 1, 1);
        }

        public bool IsValid()
        {
            return int.TryParse(Num, NumberStyles.AllowThousands, culture, out var num) && Dv.ToUpper().Equals(CalculaDv(num));
        }

        public bool IsEmpresa()
        {
            if (!IsValid()) throw new InvalidOperationException("Rut inválido");
            return int.Parse(Num, NumberStyles.AllowThousands, culture) >= inicioEmpresa;
        }

        public override string ToString()
        {
            return ToString("L");
        }

        /// <summary>
        /// Devuelve la representación de cadena correspondiente al formato
        /// </summary>
        /// <param name="format">[L]impio, [G]uión, [C]ompleto, [D]ecimal, [N]umérico</param>
        public string ToString(string format)
        {
            if (!IsValid()) throw new InvalidOperationException("Rut inválido");
            switch (format.ToUpper())
            {
                case "L":
                    return $"{int.Parse(Num, NumberStyles.AllowThousands, culture)}{Dv}";
                case "G":
                    return $"{int.Parse(Num, NumberStyles.AllowThousands, culture)}-{Dv}";
                case "C":
                    return $"{int.Parse(Num, NumberStyles.AllowThousands, culture).ToString("N0", culture)}-{Dv.ToUpper()}";
                case "D":
                    return int.Parse(Num, NumberStyles.AllowThousands, culture).ToString("D0", culture);
                case "N":
                    return int.Parse(Num, NumberStyles.AllowThousands, culture).ToString("N0", culture);
                default:
                    throw new ArgumentException("Formato inválido", nameof(format));
            }
        }

        private static string CalculaDv(int num)
        {
            var suma = 0;
            var multiplicador = 1;

            while (num != 0)
            {
                multiplicador++;
                if (multiplicador == 8)
                    multiplicador = 2;

                suma += (num % 10) * multiplicador;
                num /= 10;
            }

            suma = 11 - (suma % 11);
            if (suma == 11)
            {
                return "0";
            }
            else if (suma == 10)
            {
                return "K";
            }
            else
            {
                return suma.ToString();
            }
        }

        public static bool IsValid(string rut)
        {
            return new Rut(rut).IsValid();
        }

        public static bool IsEmpresa(string rut)
        {
            return new Rut(rut).IsEmpresa();
        }
    }
}
