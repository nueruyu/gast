using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Gast.Lib.Gaia
{
    public static class GaiaJsonSettings
    {
        public static JsonSerializerSettings Create()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new RequiredByDefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                Converters =
                {
                    new StringEnumConverter(new SnakeCaseNamingStrategy()),
                    new TaskReferenceDtoJsonConverter()
                }
            };
        }

        class RequiredByDefaultContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);

                if (property.Required == Required.Default)
                    if (member.GetCustomAttribute<JsonOptionalAttribute>() == null)
                        property.Required = Required.Always;

                return property;
            }
        }
    }
}