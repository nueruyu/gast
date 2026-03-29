using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gast.Domain.AI;
using Gast.Unity.Infrastructure.Remoting.AI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Gast.Unity.Infrastructure.JsonConverters
{
    public class AIObjectiveJsonConverter : JsonConverter
    {
        static readonly ConstructorOnlyContractResolver contractResolver = new()
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };

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

            var original = serializer.ContractResolver;
            try
            {
                serializer.ContractResolver = contractResolver;
                return (IAIObjective)parameters.ToObject(concreteType, serializer);
            }
            finally
            {
                serializer.ContractResolver = original;
            }
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Only marks constructor parameters as required; all other properties are ignored.
        /// </summary>
        class ConstructorOnlyContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var properties = base.CreateProperties(type, memberSerialization);
                var ctorParams = GetConstructorParameterNames(type);

                foreach (var property in properties)
                {
                    if (ctorParams.Contains(property.PropertyName))
                        property.Required = Required.Always;
                    else
                        property.Ignored = true;
                }

                return properties;
            }

            static HashSet<string> GetConstructorParameterNames(Type type)
            {
                var ctor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                    .OrderByDescending(c => c.GetParameters().Length)
                    .FirstOrDefault();

                if (ctor == null)
                    return new HashSet<string>();

                var namingStrategy = new SnakeCaseNamingStrategy();
                return new HashSet<string>(
                    ctor.GetParameters().Select(p => namingStrategy.GetPropertyName(p.Name, false)),
                    StringComparer.Ordinal);
            }
        }
    }
}
