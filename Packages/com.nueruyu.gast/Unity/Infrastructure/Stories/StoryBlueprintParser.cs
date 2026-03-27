using System.Linq;
using Gast.Domain.Stories;
using Gast.Lib.Gaia;
using Gast.Unity.Infrastructure.Stories.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Gast.Unity.Infrastructure.Stories
{
    /// <summary>
    /// Parses a Gaia story JSON string into a <see cref="StoryBlueprint"/> domain model.
    /// Newtonsoft deserialization is contained here; callers receive a plain domain object.
    /// </summary>
    public static class StoryBlueprintParser
    {
        static readonly JsonSerializerSettings Settings = new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            },
            Converters = { new TaskReferenceJsonConverter() }
        };

        public static StoryBlueprint Parse(string json)
        {
            var def = JsonConvert.DeserializeObject<StoryDefinition>(json, Settings);
            return Map(def);
        }

        static StoryBlueprint Map(StoryDefinition def) => new(
            def.DomainName,
            def.RootTask,
            def.Tasks.Select(MapTask).ToList()
        );

        static BlueprintTask MapTask(TaskDefinition t) => new(
            t.Name, t.Type, t.Selector,
            t.Methods?.Select(MapMethod).ToList()
        );

        static BlueprintMethod MapMethod(MethodDefinition m) => new(
            m.Name,
            m.Tasks?.Select(MapTaskRef).ToList()
        );

        static BlueprintTaskRef MapTaskRef(TaskReference r) =>
            r.IsCompound
                ? BlueprintTaskRef.ForCompound(r.CompoundTaskName)
                : BlueprintTaskRef.ForPrimitive(r.Action, r.ParametersJson);
    }
}
