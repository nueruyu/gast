using System;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Domain.Pickups;
using Newtonsoft.Json;

namespace Gast.Unity.Infrastructure.Remoting.AI
{
    public class DomainValueObjectConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(CharacterTypeId) ||
                   objectType == typeof(CharacterId) ||
                   objectType == typeof(ItemId) ||
                   objectType == typeof(PickupId);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var value = (string)reader.Value;

            if (objectType == typeof(CharacterTypeId))
            {
                return CharacterTypeId.FromString(value);
            }

            if (objectType == typeof(CharacterId))
            {
                return CharacterId.FromGuid(Guid.Parse(value));
            }

            if (objectType == typeof(ItemId))
            {
                return ItemId.FromGuid(Guid.Parse(value));
            }

            if (objectType == typeof(PickupId))
            {
                return PickupId.FromGuid(Guid.Parse(value));
            }

            throw new JsonSerializationException($"Unexpected type for DomainValueObjectConverter: {objectType}");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
