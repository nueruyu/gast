using Gast.Infrastructure.Factories;
using Gast.Infrastructure.Repositories;
using Gast.Infrastructure.Services;
using VContainer;
using VContainer.Unity;

namespace Gast.Composition.Installers
{
    public class EconomyInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // Economy System
            builder.Register<ItemRepository>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .AsSelf();

            builder.Register<ShopInitializer>(Lifetime.Singleton).AsImplementedInterfaces();

            // Pickups
            builder.Register<PickupFactory>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<PickupRepository>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}