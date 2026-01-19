using Gast.Core.DI;
using Gast.Features.AI;
using Gast.Features.Cameras;
using Gast.Features.Debugging;
using Gast.Features.Gameplay;
using Gast.Features.Gathering;
using Gast.Features.Inputs;
using Gast.Features.Installers;
using Gast.Features.Interactions;
using Gast.Features.Loot;
using Gast.Features.Players;
using Gast.Features.SpawnSites;
using Gast.Lib.AI.Debugging;

namespace Gast.Features
{
    public class FeaturesInstaller : IInstaller
    {
        readonly CombatAIInstaller combatAIInstaller = new();
        readonly StrategicAIInstaller strategicAIInstaller = new();

        public void Install(IContainerBuilder builder)
        {
            // AI
            builder.Register<GoalManager>(Lifetime.Transient);
            builder.Register<AIBrain>(Lifetime.Transient);

            combatAIInstaller.Install(builder);
            strategicAIInstaller.Install(builder);

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

            // Debugging
            builder.Register<AIDebugger>().As<IAIDebugger>();
            builder.Register<AIDebugInitializer>().AsImplementedInterfaces();
        }
    }
}