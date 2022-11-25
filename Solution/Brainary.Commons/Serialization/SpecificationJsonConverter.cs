using Brainary.Commons.Data.Patterns;
using Brainary.Commons.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Brainary.Commons.Serialization
{
    public sealed class SpecificationJsonConverter<T> : JsonConverter<Specification<T>>
    {
        public override Specification<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Specification<T> value, JsonSerializerOptions options)
        {
            var dictvalues = new Dictionary<string, object?>
            {
                { nameof(value.Criteria), value.Criteria?.Simplify().ToString() },
                { nameof(value.Includes), value.Includes.Select(s => s.Simplify().ToString()) },
                { nameof(value.IncludeStrings), value.IncludeStrings },
                { nameof(value.OrderBy), value.OrderBy?.Simplify().ToString() },
                { nameof(value.OrderByDescending), value.OrderByDescending?.Simplify().ToString() },
                { nameof(value.GroupBy), value.GroupBy?.Simplify().ToString() },
                { nameof(value.Take), value.Take },
                { nameof(value.Skip), value.Skip },
                { nameof(value.IsPagingEnabled), value.IsPagingEnabled }
            };

            var typename = value.GetType().Name;
            var dicttype = new Dictionary<string, Dictionary<string, object?>> { { typename, dictvalues } };

            var converter =  (JsonConverter<Dictionary<string, Dictionary<string, object?>>>)options.GetConverter(typeof(Dictionary<string, Dictionary<string, object?>>));
            converter.Write(writer, dicttype, options);
        }
    }
}
