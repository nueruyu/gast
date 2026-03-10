using Gast.Core.Commands;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Domain.Items;

namespace Gast.Application.Items
{
    public class PlaceItemUseCase : ICommandHandler<PlaceItemCommand, bool>
    {
        readonly ICharacterRepository characterRepository;
        readonly IItemRepository itemRepository;
        readonly IPlacedItemFactory placedItemFactory;

        public PlaceItemUseCase(
            ICharacterRepository characterRepository,
            IItemRepository itemRepository,
            IPlacedItemFactory placedItemFactory)
        {
            this.characterRepository = characterRepository;
            this.itemRepository = itemRepository;
            this.placedItemFactory = placedItemFactory;
        }

        public bool Execute(in PlaceItemCommand command)
        {
            var character = characterRepository.Get(command.PlacerId);
            if (!character.Is(out IInventoryHost inventoryHost))
            {
                return false;
            }

            var itemDefinition = itemRepository.Get(command.ItemId);

            if (!itemDefinition.Placeable)
            {
                return false;
            }

            if (!inventoryHost.Inventory.RemoveItem(command.ItemId, 1))
            {
                return false;
            }

            if (!placedItemFactory.Create(command.ItemId, command.Position, command.Rotation))
            {
                // Creation failed, return the item to the inventory
                inventoryHost.Inventory.AddItem(itemDefinition, 1);
                return false;
            }

            return true;
        }
    }
}
