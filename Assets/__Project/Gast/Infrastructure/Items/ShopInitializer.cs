using Gast.Core.Commands;
using Gast.Core.Tasks;
using Gast.Domain.Economy;
using Gast.Features.Economy;
using Gast.Features.Interactions;
using System.Threading;
using System.Threading.Tasks;

namespace Gast.Infrastructure.Items
{
    public class ShopInitializer : ILifecycleTask
    {
        readonly ShopRegistry shopRegistry;
        readonly ICommandDispatcher commandDispatcher;
        readonly IItemRepository itemRepository;
        readonly InteractionSystem interactionSystem;

        public ShopInitializer(
            ShopRegistry shopRegistry,
            ICommandDispatcher commandDispatcher,
            IItemRepository itemRepository,
            InteractionSystem interactionSystem)
        {
            this.shopRegistry = shopRegistry;
            this.commandDispatcher = commandDispatcher;
            this.itemRepository = itemRepository;
            this.interactionSystem = interactionSystem;
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            foreach (var shop in shopRegistry.GetShops())
            {
                shop.Initialize(commandDispatcher, itemRepository, interactionSystem);
            }

            return Task.CompletedTask;
        }
    }
}