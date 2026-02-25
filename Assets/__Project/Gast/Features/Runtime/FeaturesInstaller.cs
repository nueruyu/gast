using Gast.Core.DI;
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