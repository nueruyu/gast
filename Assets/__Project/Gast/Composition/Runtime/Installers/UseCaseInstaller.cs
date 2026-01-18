using Gast.Application.Characters;
using Gast.Application.Economy;
using Gast.Application.Gathering;
using Gast.Application.Interactions;
using Gast.Application.Loot;
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

            // Loot & Gathering
            builder.Register<SpawnLootUseCase>(Lifetime.Singleton)
                .AsImplementedInterfaces();
            builder.Register<SpawnGatheringItemUseCase>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            // Interactions
            builder.Register<InteractUseCase>(Lifetime.Singleton)
                .AsImplementedInterfaces();
        }
    }
}