using Gast.Lib.Gaia;
using Gast.Unity.Infrastructure.JsonConverters;
using Newtonsoft.Json;

namespace Gast.Unity.Infrastructure.Remoting.AI
{
    public class GaiaJsonSettingsFactory
    {
        readonly ObjectiveTypeResolver objectiveTypeResolver;

        public GaiaJsonSettingsFactory(ObjectiveTypeResolver objectiveTypeResolver)
        {
            this.objectiveTypeResolver = objectiveTypeResolver;
        }

        public JsonSerializerSettings Create()
        {
            var settings = GaiaJsonSettings.Create();
            settings.Converters.Add(new CharacterIdJsonConverter());
            settings.Converters.Add(new CharacterTypeIdJsonConverter());
            settings.Converters.Add(new ItemIdJsonConverter());
            settings.Converters.Add(new PickupIdJsonConverter());
            settings.Converters.Add(new AIObjectiveJsonConverter(objectiveTypeResolver));
            return settings;
        }
    }
}
