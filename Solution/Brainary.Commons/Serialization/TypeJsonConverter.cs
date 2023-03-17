using Brainary.Commons.Data.Patterns;
using Brainary.Commons.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Brainary.Commons.Serialization
{
    /// <summary>
    /// Json converter for <see cref="Type"/>
    /// </summary>
    public sealed class TypeJsonConverter : JsonConverter<Type>
    {
        public override Type? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.AssemblyQualifiedName);
        }
    }
}
