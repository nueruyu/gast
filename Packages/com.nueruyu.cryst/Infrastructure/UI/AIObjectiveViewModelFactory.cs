using Gast.Domain.AI;
using Cryst.Domain.AI.Objectives;
using System;
using VContainer;
using Cryst.UI.Hud.Objectives;
using Gast.Unity.UI.Hud.Objectives;

namespace Cryst.Infrastructure.UI
{
    public class AIObjectiveViewModelFactory : IAIObjectiveViewModelFactory
    {
        readonly IObjectResolver resolver;

        public AIObjectiveViewModelFactory(IObjectResolver resolver)
        {
            this.resolver = resolver;
        }

        public IAIObjectiveViewModel Create(IAIObjective objective)
        {
            using var scope = resolver.CreateScope(builder =>
            {
                builder.RegisterInstance(objective).AsSelf();
            });

            return objective switch
            {
                AcquireItemObjective => scope.Resolve<AcquireItemObjectiveViewModel>(),
                DefeatCharacterObjective => scope.Resolve<DefeatCharacterObjectiveViewModel>(),
                _ => throw new ArgumentException(),
            };
        }
    }
}
