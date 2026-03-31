using System;
using System.Collections.Generic;
using Gast.Application.AIPlanning;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;

namespace Gast.Unity.Features.Stories
{
    /// <summary>
    ///     Builds an <see cref="AIDomain{TActorContext,TWorldState}" /> from a <see cref="StoryBlueprint" />.
    ///     Parameters on each <see cref="IBlueprintTaskRef" /> are already fully deserialized by
    ///     <see cref="Gast.Unity.Infrastructure.Remoting.AI.StoryBlueprintMapper" />.
    /// </summary>
    public class StoryDomainFactory
    {
        public AIDomain<StoryActorContext, StoryWorldState> CreateDomain(StoryBlueprint blueprint)
        {
            var domainBuilder = new AIDomainBuilder<StoryActorContext, StoryWorldState>();

            // Pass 1: register all compound task builders so cross-references can be resolved
            var compoundBuilders = new Dictionary<string, CompoundTaskBuilder<StoryActorContext, StoryWorldState>>();
            foreach (var taskDef in blueprint.Tasks)
                compoundBuilders[taskDef.Name] = domainBuilder.DefineCompound(taskDef.Name);

            // Pass 2: populate each compound task with its methods and subtasks
            foreach (var taskDef in blueprint.Tasks)
            {
                var compoundBuilder = compoundBuilders[taskDef.Name];

                if (taskDef.Methods.Count == 0)
                    throw new InvalidOperationException(
                        $"Compound task '{taskDef.Name}' has no methods.");

                foreach (var methodDef in taskDef.Methods)
                {
                    if (methodDef.Tasks.Count == 0)
                        throw new InvalidOperationException(
                            $"Method '{methodDef.Name}' in task '{taskDef.Name}' has no tasks.");

                    var methodBuilder = compoundBuilder.AddMethod(methodDef.Name);

                    foreach (var taskRef in methodDef.Tasks)
                        switch (taskRef)
                        {
                            case CompoundTaskRef compoundRef:
                                if (!compoundBuilders.TryGetValue(
                                        compoundRef.CompoundTaskName,
                                        out var referencedBuilder))
                                    throw new InvalidOperationException(
                                        $"Unknown compound task reference: '{compoundRef.CompoundTaskName}'.");

                                methodBuilder.Do(referencedBuilder);
                                break;
                            case PrimitiveTaskRef primitiveRef:
                                methodBuilder.Do(primitiveRef.Action);
                                break;
                            default:
                                throw new InvalidOperationException(
                                    $"Unknown task reference type: {taskRef.GetType().Name}");
                        }
                }
            }

            return domainBuilder.Build(blueprint.RootTask);
        }
    }
}