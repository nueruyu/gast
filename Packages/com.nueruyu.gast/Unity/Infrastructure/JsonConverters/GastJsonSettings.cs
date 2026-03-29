using Gast.Lib.Gaia;
using Gast.Unity.Infrastructure.Remoting.AI;
using Newtonsoft.Json;

namespace Gast.Unity.Infrastructure.JsonConverters
{
    public class GastJsonSettings
    {
        readonly ObjectiveTypeResolver objectiveTypeResolver;

        public GastJsonSettings(ObjectiveTypeResolver objectiveTypeResolver)
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
