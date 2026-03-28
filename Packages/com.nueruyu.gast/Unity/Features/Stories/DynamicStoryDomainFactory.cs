using System;
using System.Collections.Generic;
using System.Linq;
using Gast.Application.AIPlanning;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;
using UnityEngine;

namespace Gast.Unity.Features.Stories
{
    /// <summary>
    /// Builds an <see cref="AIDomain{TActorContext,TWorldState}"/> from a <see cref="StoryBlueprint"/>.
    /// Parameters on each <see cref="BlueprintTaskRef"/> are already fully deserialized by
    /// </summary>
    public class DynamicStoryDomainFactory
    {
        readonly IReadOnlyDictionary<string, IStoryActionFactory> actionRegistry;

        public DynamicStoryDomainFactory(IEnumerable<IStoryActionFactory> actionFactories)
        {
            actionRegistry = actionFactories.ToDictionary(
                f => f.ActionName,
                f => f,
                StringComparer.OrdinalIgnoreCase);
        }

        public AIDomain<StoryActorContext, StoryWorldState> CreateDomain(StoryBlueprint blueprint)
        {
            var domainBuilder = new AIDomainBuilder<StoryActorContext, StoryWorldState>();

            // Pass 1: register all compound task builders so cross-references can be resolved
            var compoundBuilders = new Dictionary<string, CompoundTaskBuilder<StoryActorContext, StoryWorldState>>();
            foreach (var taskDef in blueprint.Tasks.Where(t => t.Type == "compound"))
            {
                compoundBuilders[taskDef.Name] = domainBuilder.DefineCompound(taskDef.Name);
            }

            // Pass 2: populate each compound task with its methods and subtasks
            foreach (var taskDef in blueprint.Tasks.Where(t => t.Type == "compound"))
            {
                var compoundBuilder = compoundBuilders[taskDef.Name];
                foreach (var methodDef in taskDef.Methods)
                {
                    var methodBuilder = compoundBuilder.AddMethod(methodDef.Name);
                    foreach (var taskRef in methodDef.Tasks)
                    {
                        if (taskRef.IsCompound)
                        {
                            if (!compoundBuilders.TryGetValue(taskRef.CompoundTaskName, out var referencedBuilder))
                            {
                                Debug.LogError(
                                    "[DynamicStoryDomainFactory] Unknown " +
                                    $"compound task reference: '{taskRef.CompoundTaskName}'.");
                                continue;
                            }

                            methodBuilder.Do(referencedBuilder);
                        }
                        else
                        {
                            var action = BuildPrimitiveAction(taskRef);
                            if (action != null)
                                methodBuilder.Do(action);
                        }
                    }
                }
            }

            return domainBuilder.Build(blueprint.RootTask);
        }

        IAction<StoryActorContext, StoryWorldState> BuildPrimitiveAction(BlueprintTaskRef taskRef)
        {
            if (string.IsNullOrEmpty(taskRef.ActionName))
            {
                Debug.LogError("[DynamicStoryDomainFactory] Primitive task is missing 'action' field.");
                return null;
            }

            if (!actionRegistry.TryGetValue(taskRef.ActionName, out var factory))
            {
                Debug.LogError($"[DynamicStoryDomainFactory] No factory registered for action '{taskRef.ActionName}'.");
                return null;
            }

            return factory.Create(taskRef.Parameters);
        }
    }
}