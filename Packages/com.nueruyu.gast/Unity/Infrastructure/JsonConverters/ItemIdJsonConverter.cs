using System;
using Gast.Domain.Economy;
using Newtonsoft.Json;

namespace Gast.Unity.Infrastructure.JsonConverters
{
    public class ItemIdJsonConverter : JsonConverter<ItemId>
    {
        public override ItemId ReadJson(
            JsonReader reader, Type objectType,
            ItemId existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
                throw new JsonSerializationException(
                    $"Expected string for ItemId, got {reader.TokenType}.");

            return ItemId.FromString((string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, ItemId value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
