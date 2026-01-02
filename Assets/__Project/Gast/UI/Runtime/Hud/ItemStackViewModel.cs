using Gast.Domain.Economy;

namespace Gast.UI.Hud
{
    public class ItemStackViewModel
    {
        readonly ItemStack itemStack;
        readonly IItemDefinition itemDefinition;

        public ItemStackViewModel(ItemStack itemStack, IItemDefinition itemDefinition)
        {
            this.itemStack = itemStack;
            this.itemDefinition = itemDefinition;
        }

        public ItemStack ItemStack => itemStack;

        public IItemDefinition ItemDefinition => itemDefinition;
    }
}