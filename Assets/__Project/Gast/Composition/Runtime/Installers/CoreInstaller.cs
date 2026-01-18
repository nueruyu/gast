using Gast.Application.CommandHandlers;
using Gast.Features.Cameras;
using Gast.Features.Gameplay;
using Gast.Features.Inputs;
using Gast.Features.Players;
using Gast.Infrastructure.Remoting.AI;
using Gast.Infrastructure.Services;
using Gast.UI.System;
using VContainer;
using VContainer.Unity;

namespace Gast.Composition.Installers
{
    public class CoreInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // Command System
            builder.Register<CommandDispatcher>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<JsonCommandSerializer>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<CharacterCommandHandler>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<EconomyCommandHandler>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<PickupCommandHandler>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<InteractionCommandHandler>(Lifetime.Singleton).AsImplementedInterfaces();

            // Domain Event System
            builder.Register<DomainEventPublisher>(Lifetime.Singleton).AsImplementedInterfaces();

            // AI Server Client
            builder.Register<AIAgentService>(Lifetime.Singleton).AsImplementedInterfaces();

            // Input system
            builder.Register<InputReader>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<InputModeManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<InputModeController>(Lifetime.Singleton).AsImplementedInterfaces();

            // Camera
            builder.Register<CameraService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<CameraInputController>(Lifetime.Singleton).AsImplementedInterfaces();

            // Player
            builder.Register<PlayerBrain>(Lifetime.Singleton);
            builder.Register<PlayerManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<PlayerInteractionFocusService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            // Lifecycle tasks
            builder.Register<PlayerContextBinder>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.RegisterEntryPoint<LifecycleTaskRunner>();

            // Game initialization
            builder.RegisterEntryPoint<GameInitializer>();
        }
    }
}