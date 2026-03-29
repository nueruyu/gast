using System;
using System.Linq;
using Gast.Application.AIPlanning;
using Gast.Lib.AI;
using Gast.Lib.Gaia;
using Gast.Unity.Features.Stories;
using Gast.Unity.Infrastructure.AI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                t.Methods?.Select(MapMethod).ToList()
            );
        }

        BlueprintMethod MapMethod(MethodDefinitionDto m)
        {
            return new BlueprintMethod(
                m.Name,
                m.Tasks.Select(MapTaskRef).ToList()
            );
        }

        IBlueprintTaskRef MapTaskRef(TaskReferenceDto r)
        {
            if (r.IsCompound)
                return new CompoundTaskRef(r.CompoundTaskName);

            return new PrimitiveTaskRef(BuildPrimitiveAction(r));
        }

        IAction<StoryActorContext, StoryWorldState> BuildPrimitiveAction(TaskReferenceDto taskRef)
        {
            var actionType = actionTypeResolver.Resolve(taskRef.Action);
            var jObj = taskRef.Parameters != null
                ? JObject.FromObject(taskRef.Parameters)
                : new JObject();
            return (IAction<StoryActorContext, StoryWorldState>)jObj.ToObject(actionType, parameterSerializer);
        }
    }
}