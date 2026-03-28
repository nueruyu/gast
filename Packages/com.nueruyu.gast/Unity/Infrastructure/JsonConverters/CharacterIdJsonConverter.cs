using System;
using Gast.Domain.Characters;
using Newtonsoft.Json;

namespace Gast.Unity.Infrastructure.JsonConverters
{
    public class CharacterIdJsonConverter : JsonConverter<CharacterId>
    {
        public override CharacterId ReadJson(
            JsonReader reader, Type objectType,
            CharacterId existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
                throw new JsonSerializationException(
                    $"Expected string for CharacterId, got {reader.TokenType}.");

            return CharacterId.FromGuid(Guid.Parse((string)reader.Value));
        }

        public override void WriteJson(JsonWriter writer, CharacterId value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
