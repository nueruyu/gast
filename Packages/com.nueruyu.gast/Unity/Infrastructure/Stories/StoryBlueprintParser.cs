using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gast.Application.AIPlanning;
using Gast.Lib.AI;
using Gast.Lib.Gaia;
using Gast.Unity.Features.Stories;
using Gast.Unity.Infrastructure.AI;
using Gast.Unity.Infrastructure.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Gast.Unity.Infrastructure.Stories
{
    public class StoryBlueprintParser
    {
        readonly AIActionTypeResolver actionTypeResolver;
        readonly JsonSerializer parameterSerializer;
        readonly JsonSerializerSettings definitionSettings;

        public StoryBlueprintParser(AIActionTypeResolver actionTypeResolver)
        {
            this.actionTypeResolver = actionTypeResolver;

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

            try
            {
                var actionType = actionTypeResolver.Resolve(taskRef.Action);
                var json = taskRef.ParametersJson ?? "{}";
                using var reader = new JsonTextReader(new StringReader(json));
                var actionInstance = parameterSerializer.Deserialize(reader, actionType);
                return (IAction<StoryActorContext, StoryWorldState>)actionInstance;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[StoryBlueprintParser] Failed to create action '{taskRef.Action}': {ex.Message}");
                return null;
            }
        }
    }
}
