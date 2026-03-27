using System.Collections.Generic;
using System.Linq;
using Cryst.Features.Stories.Converters;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;
using Gast.Lib.Gaia;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Cryst.Features.Stories
{
    /// <summary>
    /// Parses a story definition JSON string and builds an <see cref="AIDomain{TActorContext,TWorldState}"/>
    /// using <see cref="AIDomainBuilder{TActorContext,TWorldState}"/>.
    ///
    /// All JSON token manipulation is contained here. Action parameters are deserialized to typed
    /// objects (using <see cref="IStoryActionFactory.ParameterType"/>) before being passed to each
    /// factory — action classes themselves remain JSON-free.
    /// </summary>
    public class DynamicStoryDomainFactory
    {
        readonly IReadOnlyDictionary<string, IStoryActionFactory> actionRegistry;
        readonly JsonSerializer parameterSerializer;

        public DynamicStoryDomainFactory(IEnumerable<IStoryActionFactory> actionFactories)
        {
            actionRegistry = actionFactories.ToDictionary(
                f => f.ActionName,
                f => f,
                System.StringComparer.OrdinalIgnoreCase);

            parameterSerializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                Converters =
                {
                    new CharacterIdJsonConverter(),
                    new CharacterTypeIdJsonConverter(),
                    new ItemIdJsonConverter()
                }
            });
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
                            var referencedName = taskRef.Value<string>();
                            if (!compoundBuilders.TryGetValue(referencedName, out var referencedBuilder))
                            {
                                Debug.LogError($"[DynamicStoryDomainFactory] Unknown compound task reference: '{referencedName}'.");
                                continue;
                            }
                            methodBuilder.Do(referencedBuilder);
                        }
                        else if (taskRef.Type == JTokenType.Object)
                        {
                            var action = BuildPrimitiveAction((JObject)taskRef);
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

            var parametersToken = primitiveObj["parameters"] ?? new JObject();
            var parameters = parametersToken.ToObject(factory.ParameterType, parameterSerializer);
            return factory.Create(parameters);
        }
    }
}
