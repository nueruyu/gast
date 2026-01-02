using Gast.UseCases.Characters;
using Gast.UseCases.Economy;
using Gast.UseCases.Gathering;
using Gast.UseCases.Loot;
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
        }
    }
}