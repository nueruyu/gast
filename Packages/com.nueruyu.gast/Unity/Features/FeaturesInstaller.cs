using Gast.Core.DI;
using Gast.Unity.Features.Cameras;
using Gast.Unity.Features.Characters;
using Gast.Unity.Features.Gameplay;
using Gast.Unity.Features.Gathering;
using Gast.Unity.Features.Inputs;
using Gast.Unity.Features.Interactions;
using Gast.Unity.Features.Loot;
using Gast.Unity.Features.Players;
using Gast.Unity.Features.SpawnSites;

namespace Gast.Unity.Features
{
    public class FeaturesInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // Camera
            builder.Register<CameraInputController>().AsImplementedInterfaces();
            builder.Register<CameraService>().AsImplementedInterfaces();

            // Player
            builder.Register<PlayerAIControlMonitorService>();
            builder.Register<PlayerInteractionFocusService>().AsImplementedInterfaces().AsSelf();
            builder.Register<PlayerBrain>();
            builder.Register<PlayerManager>().AsImplementedInterfaces().AsSelf();

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