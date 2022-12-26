using Brainary.Commons.Data.Patterns;
using Brainary.Commons.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Brainary.Commons.Serialization
{
    /// <summary>
    /// Json converter factory for <see cref="Specification{T}"/>
    /// </summary>
    public class SpecificationJsonConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsSubclassOfRawGeneric(typeof(Specification<>));
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var keyType = typeToConvert.BaseType!.GenericTypeArguments[0];
            var converterType = typeof(SpecificationJsonConverter<>).MakeGenericType(keyType);

            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }
    }
}
