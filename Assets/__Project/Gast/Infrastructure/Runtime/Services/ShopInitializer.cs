using Gast.Core.Commands;
using Gast.Core.Tasks;
using Gast.Domain.Economy;
using Gast.Features.Economy;
using System.Threading;
using System.Threading.Tasks;

namespace Gast.Infrastructure.Services
{
    public class ShopInitializer : ILifecycleTask
    {
        readonly ShopRegistry shopRegistry;
        readonly ICommandDispatcher commandDispatcher;
        readonly IItemRepository itemRepository;

        public ShopInitializer(ShopRegistry shopRegistry, ICommandDispatcher commandDispatcher, IItemRepository itemRepository)
        {
            this.shopRegistry = shopRegistry;
            this.commandDispatcher = commandDispatcher;
            this.itemRepository = itemRepository;
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            foreach (var shop in shopRegistry.Shops)
            {
                shop.Initialize(commandDispatcher, itemRepository);
            }

            return Task.CompletedTask;
        }
    }
}