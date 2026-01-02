using DescrioGames.Features.Gathering;
using DescrioGames.Features.Interactions;
using DescrioGames.Features.Loot;
using DescrioGames.Features.SpawnSites;
using VContainer;
using VContainer.Unity;

namespace DescrioGames.Composition.Installers
{
    public class GameplaySystemInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // Loot & Gathering System
            builder.Register<LootSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GatheringSystem>(Lifetime.Singleton).AsImplementedInterfaces();

            // Interaction system
            builder.Register<InteractionSystem>(Lifetime.Singleton)
                .AsSelf()
                .AsImplementedInterfaces();
            builder.Register<InteractionInputHandler>(Lifetime.Singleton).AsImplementedInterfaces();

            // Spawn sites
            builder.Register<SpawnSiteSystem>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}