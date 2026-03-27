using System;
using Gast.Domain.Characters;
using Newtonsoft.Json;

namespace Cryst.Features.Stories.Converters
{
    public class CharacterTypeIdJsonConverter : JsonConverter<CharacterTypeId>
    {
        public override CharacterTypeId ReadJson(
            JsonReader reader, Type objectType,
            CharacterTypeId existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
                throw new JsonSerializationException(
                    $"Expected string for CharacterTypeId, got {reader.TokenType}.");

            return CharacterTypeId.FromString((string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, CharacterTypeId value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
