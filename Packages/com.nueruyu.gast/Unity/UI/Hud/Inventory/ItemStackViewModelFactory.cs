using Gast.Domain.Economy;

namespace Gast.Unity.UI.Hud.Inventory
{
    public class ItemStackViewModelFactory
    {
        readonly IItemRepository itemRepository;

        public ItemStackViewModelFactory(IItemRepository itemRepository)
        {
            this.itemRepository = itemRepository;
        }

        public ItemStackViewModel Create(ItemStack itemStack)
        {
            var itemDefinition = itemRepository.Get(itemStack.ItemId);
            return new(itemStack, itemDefinition);
        }
    }
}