using System;
using System.Collections.Generic;
using System.Linq;
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
            foreach (var taskDef in blueprint.Tasks.Where(t => t.Type == "compound"))
                compoundBuilders[taskDef.Name] = domainBuilder.DefineCompound(taskDef.Name);

            // Pass 2: populate each compound task with its methods and subtasks
            foreach (var taskDef in blueprint.Tasks.Where(t => t.Type == "compound"))
            {
                var compoundBuilder = compoundBuilders[taskDef.Name];

                foreach (var methodDef in taskDef.Methods)
                {
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
                        }
                }
            }

            return domainBuilder.Build(blueprint.RootTask);
        }
    }
}