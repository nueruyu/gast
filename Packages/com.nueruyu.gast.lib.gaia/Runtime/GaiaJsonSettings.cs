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
                ContractResolver = new DefaultContractResolver
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
    }
}
