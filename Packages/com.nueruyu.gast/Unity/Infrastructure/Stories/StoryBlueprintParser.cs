using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gast.Application.AIPlanning;
using Gast.Lib.AI;
using Gast.Lib.Gaia;
using Gast.Unity.Features.Stories;
using Gast.Unity.Infrastructure.Stories.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

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
        readonly IReadOnlyDictionary<string, IStoryActionFactory> actionRegistry;
        readonly JsonSerializer parameterSerializer;
        readonly JsonSerializerSettings definitionSettings;

        public StoryBlueprintParser(IEnumerable<IStoryActionFactory> actionFactories)
        {
            actionRegistry = actionFactories.ToDictionary(
                f => f.ActionName,
                f => f,
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
            m.Tasks?.Select(MapTaskRef).Where(x => x != null).ToList()
        );

        IBlueprintTaskRef MapTaskRef(TaskReference r)
        {
            if (r.IsCompound)
                return new CompoundTaskRef(r.CompoundTaskName);

            var action = BuildPrimitiveAction(r);
            if (action == null) 
                return null;

            return new PrimitiveTaskRef(action);
        }

        IAction<StoryActorContext, StoryWorldState> BuildPrimitiveAction(TaskReference taskRef)
        {
            if (string.IsNullOrEmpty(taskRef.Action))
            {
                Debug.LogError("[StoryBlueprintParser] Primitive task is missing 'action' field.");
                return null;
            }

            if (!actionRegistry.TryGetValue(taskRef.Action, out var factory))
            {
                Debug.LogError($"[StoryBlueprintParser] No factory registered for action '{taskRef.Action}'.");
                return null;
            }

            var json = taskRef.ParametersJson ?? "{}";
            using var reader = new JsonTextReader(new StringReader(json));
            var parameters = parameterSerializer.Deserialize(reader, factory.ParameterType);

            return factory.Create(parameters);
        }
    }
}
