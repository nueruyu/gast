using System;
using Gast.Domain.Pickups;
using Newtonsoft.Json;

namespace Gast.Unity.Infrastructure.JsonConverters
{
    public class PickupIdJsonConverter : JsonConverter<PickupId>
    {
        public override PickupId ReadJson(
            JsonReader reader, Type objectType,
            PickupId existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
                throw new JsonSerializationException(
                    $"Expected string for PickupId, got {reader.TokenType}.");

            return PickupId.FromString((string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, PickupId value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
