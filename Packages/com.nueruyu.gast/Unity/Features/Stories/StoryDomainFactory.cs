using System.Collections.Generic;
using System.Linq;
using Gast.Application.AIPlanning;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;
using UnityEngine;

namespace Gast.Unity.Features.Stories
{
    /// <summary>
    ///     Builds an <see cref="AIDomain{TActorContext,TWorldState}" /> from a <see cref="StoryBlueprint" />.
    ///     Parameters on each <see cref="IBlueprintTaskRef" /> are already fully deserialized by
    /// </summary>
    public class StoryDomainFactory
    {
        public AIDomain<StoryActorContext, StoryWorldState> CreateDomain(StoryBlueprint blueprint)
        {
            var domainBuilder = new AIDomainBuilder<StoryActorContext, StoryWorldState>();

            // Pass 1: register all compound task builders so cross-references can be resolved
            var compoundBuilders = new Dictionary<string, CompoundTaskBuilder<StoryActorContext, StoryWorldState>>();
            foreach (var taskDef in blueprint.Tasks.Where(t => t.Type == "compound"))
                compoundBuilders[taskDef.Name] = domainBuilder.DefineCompound(taskDef.Name);

            // Pass 2: populate each compound task with its methods and subtasks
            foreach (var taskDef in blueprint.Tasks.Where(t => t.Type == "compound"))
            {
                var compoundBuilder = compoundBuilders[taskDef.Name];
                if (taskDef.Methods == null)
                {
                    Debug.LogError(
                        $"[StoryDomainFactory] Methods is null. task: {taskDef.Name}");
                    continue;
                }

                if (taskDef.Methods.Count == 0)
                {
                    Debug.LogError(
                        $"[StoryDomainFactory] Methods is empty. task: {taskDef.Name}");
                    continue;
                }

                foreach (var methodDef in taskDef.Methods)
                {
                    if (methodDef.Tasks == null || methodDef.Tasks.Count == 0)
                    {
                        Debug.LogWarning(
                            $"[StoryDomainFactory] Skipping method '{methodDef.Name}' " +
                            $"with no tasks (task: {taskDef.Name}).");
                        continue;
                    }

                    var methodBuilder = compoundBuilder.AddMethod(methodDef.Name);

                    foreach (var taskRef in methodDef.Tasks)
                        switch (taskRef)
                        {
                            case CompoundTaskRef compoundRef:
                                if (!compoundBuilders.TryGetValue(
                                        compoundRef.CompoundTaskName,
                                        out var referencedBuilder))
                                {
                                    Debug.LogError(
                                        "[DynamicStoryDomainFactory] Unknown " +
                                        $"compound task reference: '{compoundRef.CompoundTaskName}'.");
                                    continue;
                                }

                                methodBuilder.Do(referencedBuilder);
                                break;
                            case PrimitiveTaskRef primitiveRef:
                                if (primitiveRef.Action == null)
                                    Debug.LogError(
                                        $"[DynamicStoryDomainFactory] Action is null. method: {methodDef.Name}, " +
                                        $"task: {primitiveRef}");
                                else
                                    methodBuilder.Do(primitiveRef.Action);
                                break;
                            default:
                                Debug.LogError(
                                    $"[DynamicStoryDomainFactory] Unknown task {taskRef?.GetType()}");
                                break;
                        }
                }
            }

            return domainBuilder.Build(blueprint.RootTask);
        }
    }
}