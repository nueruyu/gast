using System;
using Gast.Domain.AI;
using Gast.Unity.Infrastructure.Remoting.AI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gast.Unity.Infrastructure.JsonConverters
{
    public class AIObjectiveJsonConverter : JsonConverter
    {
        readonly ObjectiveTypeResolver typeResolver;

        public AIObjectiveJsonConverter(ObjectiveTypeResolver typeResolver)
        {
            this.typeResolver = typeResolver;
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(IAIObjective);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jObject = JObject.Load(reader);

            var typeName = jObject.Value<string>("type");
            if (string.IsNullOrEmpty(typeName))
                throw new JsonSerializationException("IAIObjective JSON must contain a 'type' field.");

            var concreteType = typeResolver.Resolve(typeName);

            var parameters = jObject["parameters"] as JObject ?? new JObject();
            return (IAIObjective)parameters.ToObject(concreteType, serializer);
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException("Serialization of IAIObjective is not supported.");
        }
    }
}
