using Gast.Application.UseCases.Characters;
using Gast.Application.UseCases.Economy;
using Gast.Application.UseCases.Gathering;
using Gast.Application.UseCases.Interactions;
using Gast.Application.UseCases.Loot;
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
            builder.Register<CreatePlayerUseCase>(Lifetime.Singleton);
            builder.Register<CreateNpcUseCase>(Lifetime.Singleton);

            // Economy
            builder.Register<BuyItemUseCase>(Lifetime.Singleton);
            builder.Register<PickUpItemUseCase>(Lifetime.Singleton);

            // Loot & Gathering
            builder.Register<SpawnLootUseCase>(Lifetime.Singleton);
            builder.Register<SpawnGatheringItemUseCase>(Lifetime.Singleton);

            // Interactions
            builder.Register<InteractUseCase>(Lifetime.Singleton);
        }
    }
}