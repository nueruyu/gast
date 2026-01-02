using DescrioGames.UseCases.Characters;
using DescrioGames.UseCases.Economy;
using DescrioGames.UseCases.Gathering;
using DescrioGames.UseCases.Loot;
using VContainer;
using VContainer.Unity;

namespace DescrioGames.Composition.Installers
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