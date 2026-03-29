using System;
using System.Linq;
using Gast.Application.AIPlanning;
using Gast.Lib.AI;
using Gast.Lib.Gaia;
using Gast.Unity.Features.Stories;
using Gast.Unity.Infrastructure.AI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Gast.Unity.Infrastructure.Remoting.AI
{
    public class StoryBlueprintMapper
    {
        readonly AIActionTypeResolver actionTypeResolver;
        readonly JsonSerializer parameterSerializer;

        public StoryBlueprintMapper(
            AIActionTypeResolver actionTypeResolver,
            GaiaJsonSettingsFactory jsonSettingsFactory)
        {
            this.actionTypeResolver = actionTypeResolver;
            parameterSerializer = JsonSerializer.Create(jsonSettingsFactory.Create());
        }

        public StoryBlueprint Map(StoryDefinitionDto def)
        {
            return new StoryBlueprint(
                def.RootTask,
                def.Tasks.Select(MapTask).ToList()
            );
        }

        BlueprintTask MapTask(TaskDefinitionDto t)
        {
            return new BlueprintTask(
                t.Name, t.Type, t.Selector,
                t.Methods.Select(MapMethod).ToList()
            );
        }

        BlueprintMethod MapMethod(MethodDefinitionDto m)
        {
            var taskRefs = m.Tasks.Select(MapTaskRef).Where(x => x != null).ToList();

            if (m.Tasks.Count > 0 && taskRefs.Count == 0)
                Debug.LogWarning(
                    $"[StoryBlueprintMapper] All tasks in method '{m.Name}' failed to map " +
                    $"({m.Tasks.Count} task(s) were filtered out).");

            return new BlueprintMethod(m.Name, taskRefs);
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
                Debug.LogException(ex);
                return null;
            }
        }
    }
}