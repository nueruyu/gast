using Gast.Lib.Gaia;
using Newtonsoft.Json;

namespace Gast.Unity.Infrastructure.JsonConverters
{
    public static class GastJsonSettings
    {
        public static JsonSerializerSettings Create()
        {
            var settings = GaiaJsonSettings.Create();
            settings.Converters.Add(new CharacterIdJsonConverter());
            settings.Converters.Add(new CharacterTypeIdJsonConverter());
            settings.Converters.Add(new ItemIdJsonConverter());
            settings.Converters.Add(new PickupIdJsonConverter());
            return settings;
        }
    }
}
