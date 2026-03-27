using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gast.Domain.Stories;
using Gast.Lib.Gaia;
using Gast.Unity.Features.Stories;
using Gast.Unity.Infrastructure.Stories.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Gast.Unity.Infrastructure.Stories
{
    /// <summary>
    /// Parses a Gaia story JSON string into a <see cref="StoryBlueprint"/> domain model.
    /// All Newtonsoft knowledge is contained here: structural deserialization (StoryDefinition),
    /// action parameter deserialization (typed via <see cref="IStoryActionFactory.ParameterType"/>),
    /// and value-object converters.
    /// </summary>
    public class StoryBlueprintParser
    {
        readonly IReadOnlyDictionary<string, Type> actionParameterTypes;
        readonly JsonSerializer parameterSerializer;
        readonly JsonSerializerSettings definitionSettings;

        public StoryBlueprintParser(IEnumerable<IStoryActionFactory> actionFactories)
        {
            actionParameterTypes = actionFactories.ToDictionary(
                f => f.ActionName,
                f => f.ParameterType,
                StringComparer.OrdinalIgnoreCase);

            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };

            parameterSerializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Converters =
                {
                    new CharacterIdJsonConverter(),
                    new CharacterTypeIdJsonConverter(),
                    new ItemIdJsonConverter()
                }
            });

            definitionSettings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Converters = { new TaskReferenceJsonConverter() }
            };
        }

        public StoryBlueprint Parse(string json)
        {
            var def = JsonConvert.DeserializeObject<StoryDefinition>(json, definitionSettings);
            return Map(def);
        }

        StoryBlueprint Map(StoryDefinition def) => new(
            def.DomainName,
            def.RootTask,
            def.Tasks.Select(MapTask).ToList()
        );

        BlueprintTask MapTask(TaskDefinition t) => new(
            t.Name, t.Type, t.Selector,
            t.Methods?.Select(MapMethod).ToList()
        );

        BlueprintMethod MapMethod(MethodDefinition m) => new(
            m.Name,
            m.Tasks?.Select(MapTaskRef).ToList()
        );

        BlueprintTaskRef MapTaskRef(TaskReference r)
        {
            if (r.IsCompound)
                return BlueprintTaskRef.ForCompound(r.CompoundTaskName);

            object parameters = null;
            if (actionParameterTypes.TryGetValue(r.Action, out var paramType))
            {
                var json = r.ParametersJson ?? "{}";
                using var reader = new JsonTextReader(new StringReader(json));
                parameters = parameterSerializer.Deserialize(reader, paramType);
            }

            return BlueprintTaskRef.ForPrimitive(r.Action, parameters);
        }
    }
}
