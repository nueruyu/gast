using System;
using System.Linq;
using Gast.Application.AIPlanning;
using Gast.Lib.AI;
using Gast.Lib.Gaia;
using Gast.Unity.Features.Stories;
using Gast.Unity.Infrastructure.AI;
using Gast.Unity.Infrastructure.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Gast.Unity.Infrastructure.Stories
{
    public class StoryBlueprintMapper
    {
        readonly AIActionTypeResolver actionTypeResolver;
        readonly JsonSerializer parameterSerializer;

        public StoryBlueprintMapper(AIActionTypeResolver actionTypeResolver)
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
        }

        public StoryBlueprint Map(StoryDefinitionDto def)
        {
            return new StoryBlueprint(
                def.DomainName,
                def.RootTask,
                def.Tasks.Select(MapTask).ToList()
            );
        }

        BlueprintTask MapTask(TaskDefinitionDto t)
        {
            return new BlueprintTask(
                t.Name, t.Type, t.Selector,
                t.Methods?.Select(MapMethod).ToList()
            );
        }

        BlueprintMethod MapMethod(MethodDefinitionDto m)
        {
            return new BlueprintMethod(
                m.Name,
                m.Tasks?.Select(MapTaskRef).Where(x => x != null).ToList()
            );
        }

        IBlueprintTaskRef MapTaskRef(TaskReferenceDto r)
        {
            if (r.IsCompound)
                return new CompoundTaskRef(r.CompoundTaskName);

            var action = BuildPrimitiveAction(r);
            if (action == null)
                return null;

            return new PrimitiveTaskRef(action);
        }

        IAction<StoryActorContext, StoryWorldState> BuildPrimitiveAction(TaskReferenceDto taskRef)
        {
            if (string.IsNullOrEmpty(taskRef.Action))
            {
                Debug.LogError("[StoryBlueprintMapper] Primitive task is missing 'action' field.");
                return null;
            }

            try
            {
                var actionType = actionTypeResolver.Resolve(taskRef.Action);
                var jObj = taskRef.Parameters != null
                    ? JObject.FromObject(taskRef.Parameters)
                    : new JObject();
                var actionInstance = jObj.ToObject(actionType, parameterSerializer);
                return (IAction<StoryActorContext, StoryWorldState>)actionInstance;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[StoryBlueprintMapper] Failed to create action '{taskRef.Action}': {ex.Message}");
                return null;
            }
        }
    }
}