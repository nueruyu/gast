using Cryst.Features.CharacterAI.Humanoid.Combat;
using Cryst.Features.CharacterAI.Humanoid.Gathering;
using Cryst.Features.CharacterAI.Humanoid.Objective;
using Cryst.Features.CharacterAI.Humanoid.Patrol;
using Cryst.Features.CharacterAI.Humanoid.Strategic;
using Gast.Core.DI;

namespace Cryst.Features.CharacterAI.Humanoid
{
    public class HumanoidInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // Query Service
            builder.Register<ObjectiveQueries>(Lifetime.Singleton).As<IObjectiveQueries>();

            // Strategic Domain
            builder.Register<StrategicDomainFactory>(Lifetime.Singleton).As<IAIDomainFactory<StrategicState>>();
            builder.Register<StrategicWorldStateUpdater>(Lifetime.Singleton).As<IWorldStateUpdater<StrategicState>>();
            builder.Register<StrategicDomainConstruct>(Lifetime.Singleton);

            // Objective Domain
            builder.Register<ObjectiveWorldStateUpdater>(Lifetime.Singleton).As<IWorldStateUpdater<ObjectiveState>>();
            builder.Register<ObjectiveDomainConstruct>(Lifetime.Singleton);

            // Combat Domain
            builder.Register<CombatDomainFactory>(Lifetime.Singleton).As<IAIDomainFactory<CombatState>>();
            builder.Register<CombatWorldStateUpdater>(Lifetime.Singleton).As<IWorldStateUpdater<CombatState>>();
            builder.Register<CombatDomainConstruct>(Lifetime.Singleton);

            // Gathering Domain
            builder.Register<GatheringDomainFactory>(Lifetime.Singleton).As<IAIDomainFactory<GatheringState>>();
            builder.Register<GatheringWorldStateUpdater>(Lifetime.Singleton).As<IWorldStateUpdater<GatheringState>>();
            builder.Register<GatheringDomainConstruct>(Lifetime.Singleton);

            // Patrol Domain
            builder.Register<PatrolDomainFactory>(Lifetime.Singleton).As<IAIDomainFactory<PatrolState>>();
            builder.Register<PatrolWorldStateUpdater>(Lifetime.Singleton).As<IWorldStateUpdater<PatrolState>>();
            builder.Register<PatrolDomainConstruct>(Lifetime.Singleton);

            // Brain
            builder.Register<HumanoidAIBrain>(Lifetime.Transient);
        }
    }
}
