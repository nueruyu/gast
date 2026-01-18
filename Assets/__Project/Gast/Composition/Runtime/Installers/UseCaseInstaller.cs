using Gast.Application.AI;
using Gast.Application.Characters;
using Gast.Application.Economy;
using Gast.Application.Interactions;
using Gast.Application.Items;
using VContainer;
using VContainer.Unity;

namespace Gast.Composition.Installers
{
    public class UseCaseInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // Characters
            builder.Register<SpawnCharacterUseCase>(Lifetime.Singleton);
            builder.Register<CreatePlayerUseCase>(Lifetime.Singleton)
                .AsImplementedInterfaces();
            builder.Register<CreateNpcUseCase>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            // Economy
            builder.Register<BuyItemUseCase>(Lifetime.Singleton)
                .AsImplementedInterfaces();
            builder.Register<PickUpItemUseCase>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            // Items
            builder.Register<SpawnItemUseCase>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            // Interactions
            builder.Register<InteractUseCase>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            // AI
            builder.Register<CommandAIUseCase>(Lifetime.Singleton)
                .AsImplementedInterfaces();
        }
    }
}