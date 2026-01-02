using Gast.Api.CommandHandlers;
using Gast.Core.Commands;
using Gast.Core.Tasks;
using Gast.Domain.Inputs;
using Gast.Features.Cameras;
using Gast.Features.Gameplay;
using Gast.Features.Inputs;
using Gast.Features.Players;
using Gast.Infrastructure.Services;
using VContainer;
using VContainer.Unity;

namespace Gast.Composition.Installers
{
    public class CoreInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // Command System
            builder.Register<CommandDispatcher>(Lifetime.Singleton).As<ICommandDispatcher>();
            builder.Register<JsonCommandSerializer>(Lifetime.Singleton).As<ICommandSerializer>();

            builder.Register<CharacterCommandHandler>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<EconomyCommandHandler>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<PickupCommandHandler>(Lifetime.Singleton).AsImplementedInterfaces();

            // Input system
            builder.Register<InputReader>(Lifetime.Singleton)
                .As<IInputProvider>()
                .As<ILifecycleTask>();

            // Camera
            builder.Register<CameraService>(Lifetime.Singleton).AsImplementedInterfaces();

            // Player
            builder.Register<PlayerBrain>(Lifetime.Singleton);
            builder.Register<PlayerManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            // Lifecycle tasks
            builder.Register<PlayerContextBinder>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.RegisterEntryPoint<LifecycleTaskRunner>();

            // Game initialization
            builder.RegisterEntryPoint<GameInitializer>();
        }
    }
}