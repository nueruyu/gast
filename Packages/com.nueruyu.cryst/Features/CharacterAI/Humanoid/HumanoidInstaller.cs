using Gast.Core.DI;
using Cryst.Features.CharacterAI.Humanoid.Combat;
using Cryst.Features.CharacterAI.Humanoid.Gathering;
using Cryst.Features.CharacterAI.Humanoid.Strategic;

namespace Cryst.Features.CharacterAI.Humanoid
{
    public class HumanoidInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // Combat Domain
            builder.Register<CombatDomainFactory>(Lifetime.Singleton).As<IAIDomainFactory<CombatState>>();
            builder.Register<CombatWorldStateUpdater>(Lifetime.Singleton).As<IWorldStateUpdater<CombatState>>();
            builder.Register<CombatDomainConstruct>(Lifetime.Singleton);

            // Strategic Domain
            builder.Register<StrategicDomainFactory>(Lifetime.Singleton).As<IAIDomainFactory<StrategicState>>();
            builder.Register<StrategicWorldStateUpdater>(Lifetime.Singleton).As<IWorldStateUpdater<StrategicState>>();
            builder.Register<StrategicDomainConstruct>(Lifetime.Singleton);

            // Gathering Domain
            builder.Register<GatheringDomainFactory>(Lifetime.Singleton).As<IAIDomainFactory<GatheringState>>();
            builder.Register<GatheringWorldStateUpdater>(Lifetime.Singleton).As<IWorldStateUpdater<GatheringState>>();
            builder.Register<GatheringDomainConstruct>(Lifetime.Singleton);
            
            // Brain
            builder.Register<HumanoidAIBrain>(Lifetime.Transient);
        }
    }
}
