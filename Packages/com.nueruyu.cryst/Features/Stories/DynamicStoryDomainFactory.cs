using System.Collections.Generic;
using System.Linq;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;
using Gast.Lib.Gaia;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Cryst.Features.Stories
{
    /// <summary>
    /// Parses a story definition JSON string and builds an <see cref="AIDomain{TActorContext,TWorldState}"/>
    /// using <see cref="AIDomainBuilder{TActorContext,TWorldState}"/>.
    /// All JSON token manipulation is contained here; action classes are JSON-free.
    /// </summary>
    public class DynamicStoryDomainFactory
    {
        readonly IReadOnlyDictionary<string, IStoryActionFactory> actionRegistry;

        public DynamicStoryDomainFactory(IEnumerable<IStoryActionFactory> actionFactories)
        {
            actionRegistry = actionFactories.ToDictionary(
                f => f.ActionName,
                f => f,
                System.StringComparer.OrdinalIgnoreCase);
        }

        public AIDomain<StoryActorContext, StoryWorldState> CreateDomain(string storyJson)
        {
            var definition = JsonConvert.DeserializeObject<StoryDefinition>(storyJson);
            var domainBuilder = new AIDomainBuilder<StoryActorContext, StoryWorldState>();

            // Pass 1: register all compound task builders so cross-references can be resolved
            var compoundBuilders = new Dictionary<string, CompoundTaskBuilder<StoryActorContext, StoryWorldState>>();
            foreach (var taskDef in definition.Tasks.Where(t => t.Type == "compound"))
            {
                compoundBuilders[taskDef.Name] = domainBuilder.DefineCompound(taskDef.Name);
            }

            // Pass 2: populate each compound task with its methods and subtasks
            foreach (var taskDef in definition.Tasks.Where(t => t.Type == "compound"))
            {
                var compoundBuilder = compoundBuilders[taskDef.Name];
                foreach (var methodDef in taskDef.Methods)
                {
                    var methodBuilder = compoundBuilder.AddMethod(methodDef.Name);
                    foreach (var taskRef in methodDef.TaskReferences)
                    {
                        if (taskRef.Type == JTokenType.String)
                        {
                            // Reference to another compound task by name
                            var referencedName = taskRef.Value<string>();
                            if (!compoundBuilders.TryGetValue(referencedName, out var referencedBuilder))
                            {
                                Debug.LogError($"[DynamicStoryDomainFactory] Compound task '{referencedName}' not found.");
                                continue;
                            }
                            methodBuilder.Do(referencedBuilder);
                        }
                        else if (taskRef.Type == JTokenType.Object)
                        {
                            // Inline primitive task definition
                            var primitiveObj = (JObject)taskRef;
                            var action = BuildPrimitiveAction(primitiveObj);
                            if (action != null)
                                methodBuilder.Do(action);
                        }
                    }
                }
            }

            return domainBuilder.Build(definition.RootTask);
        }

        IAction<StoryActorContext, StoryWorldState> BuildPrimitiveAction(JObject primitiveObj)
        {
            var actionName = primitiveObj["action"]?.Value<string>();
            if (string.IsNullOrEmpty(actionName))
            {
                Debug.LogError("[DynamicStoryDomainFactory] Primitive task is missing 'action' field.");
                return null;
            }

            if (!actionRegistry.TryGetValue(actionName, out var factory))
            {
                Debug.LogError($"[DynamicStoryDomainFactory] No factory registered for action '{actionName}'.");
                return null;
            }

            var parametersToken = primitiveObj["parameters"];
            var parametersJson = parametersToken?.ToString(Formatting.None) ?? "{}";
            return factory.Create(parametersJson);
        }
    }
}
