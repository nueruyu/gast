using Gast.Core.DI;
using Gast.Features.AI;
using Gast.Features.Cameras;
using Gast.Features.Gameplay;
using Gast.Features.Gathering;
using Gast.Features.Inputs;
using Gast.Features.Installers;
using Gast.Features.Interactions;
using Gast.Features.Characters;
using Gast.Features.Loot;
using Gast.Features.Players;
using Gast.Features.SpawnSites;

namespace Gast.Features
{
    public class FeaturesInstaller : IInstaller
    {
        readonly CombatAIInstaller combatAIInstaller = new();
        readonly StrategicAIInstaller strategicAIInstaller = new();
        readonly GatheringAIInstaller gatheringAIInstaller = new();

        public void Install(IContainerBuilder builder)
        {
            // AI
            builder.Register<ObjectiveManager>(Lifetime.Transient);
            builder.Register<AIBrain>(Lifetime.Transient);

            combatAIInstaller.Install(builder);
            strategicAIInstaller.Install(builder);
            gatheringAIInstaller.Install(builder);

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