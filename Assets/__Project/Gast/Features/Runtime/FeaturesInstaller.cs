using Gast.Core.DI;
using Gast.Features.AI;
using Gast.Features.AI.Combat;
using Gast.Features.AI.Combat.Actions;
using Gast.Features.AI.Strategic;
using Gast.Features.AI.Strategic.Actions;
using Gast.Features.Cameras;
using Gast.Features.Gameplay;
using Gast.Features.Gathering;
using Gast.Features.Inputs;
using Gast.Features.Interactions;
using Gast.Features.Loot;
using Gast.Features.Players;
using Gast.Features.SpawnSites;

namespace Gast.Features
{
    public class FeaturesInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // AI
            builder.Register<GoalManager>(Lifetime.Transient);
            builder.Register<ChaseTargetAction>(Lifetime.Transient);
            builder.Register<MeleeAttackAction>(Lifetime.Transient);
            builder.Register<BackOffAction>(Lifetime.Transient);
            builder.Register<StrafeAction>(Lifetime.Transient);
            builder.Register<ClearTargetAction>(Lifetime.Transient);
            builder.Register<FindTargetForGoalAction>(Lifetime.Transient);
            builder.Register<SelectThreatAction>(Lifetime.Transient);
            builder.Register<FindItemPickupAction>(Lifetime.Transient);
            builder.Register<MoveToInteractableAction>(Lifetime.Transient);
            builder.Register<InteractWithTargetAction>(Lifetime.Transient);
            builder.Register<ClearInteractableTargetAction>(Lifetime.Transient);
            builder.Register<StrategicDomain>(Lifetime.Transient);
            builder.Register<CombatDomain>(Lifetime.Transient);
            builder.Register<AIBrain>(Lifetime.Transient);

            // Camera
            builder.Register<CameraInputController>().AsImplementedInterfaces();
            builder.Register<CameraService>().AsImplementedInterfaces();

            // Player
            builder.Register<PlayerAIControlMonitorService>();
            builder.Register<PlayerBrain>();
            builder.Register<PlayerManager>().AsImplementedInterfaces().AsSelf();
            builder.Register<PlayerInteractionFocusService>().AsImplementedInterfaces().AsSelf();

            // Gameplay Lifecycle
            builder.Register<PlayerContextBinder>().AsImplementedInterfaces();
            builder.Register<GameInitializer>().AsImplementedInterfaces();

            // Input
            builder.Register<InputReader>().AsImplementedInterfaces();
            builder.Register<InputModeManager>().AsImplementedInterfaces();

            // Interaction
            builder.Register<InteractionSystem>().AsSelf().AsImplementedInterfaces();
            builder.Register<InteractionInputHandler>().AsImplementedInterfaces();

            // Loot & Gathering
            builder.Register<LootSystem>().AsImplementedInterfaces();
            builder.Register<GatheringSystem>().AsImplementedInterfaces();

            // Spawn Sites
            builder.Register<SpawnSiteSystem>().AsImplementedInterfaces();
        }
    }
}
