using System;
using System.Linq;
using Gast.Core.Commands;
using Gast.Core.Events;
using Gast.Domain.Characters;
using Gast.Domain.Economy;

namespace Gast.Application.Economy
{
    public class UseItemUseCase : ICommandHandler<UseItemCommand, bool>
    {
        readonly ICharacterRepository characterRepository;
        readonly IItemRepository itemRepository;
        readonly IDomainEventPublisher eventPublisher;

        public UseItemUseCase(
            ICharacterRepository characterRepository,
            IItemRepository itemRepository,
            IDomainEventPublisher eventPublisher)
        {
            this.characterRepository = characterRepository;
            this.itemRepository = itemRepository;
            this.eventPublisher = eventPublisher;
        }

        public bool Execute(in UseItemCommand command)
        {
            var character = characterRepository.Get(command.UserId);
            if (!character.Is(out IInventoryHost inventoryHost))
            {
                return false;
            }

            var inventory = inventoryHost.Inventory;
            if (command.InventorySlotIndex < 0 || command.InventorySlotIndex >= inventory.Items.Count)
            {
                return false;
            }

            var itemStack = inventory.Items[command.InventorySlotIndex];
            var itemDefinition = itemRepository.Get(itemStack.ItemId);

            if (itemDefinition.Effects == null || !itemDefinition.Effects.Any())
            {
                return false;
            }

            if (!inventory.RemoveItem(itemStack.ItemId, 1))
            {
                return false;
            }

            foreach (var effect in itemDefinition.Effects)
            {
                effect.Apply(character);
            }

            eventPublisher.Publish(new ItemUsedEvent(command.UserId, itemStack.ItemId));
            return true;
        }
    }
}
