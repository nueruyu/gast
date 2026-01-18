using Gast.Core.Events;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Domain.Events;

namespace Gast.Application.UseCases.Economy
{
    /// <summary>
    /// Use case for picking up items and adding them to inventory.
    /// </summary>
    public class PickUpItemUseCase
    {
        readonly ICharacterRepository characterRepository;
        readonly IItemRepository itemRepository;
        readonly IDomainEventPublisher eventPublisher;

        public PickUpItemUseCase(ICharacterRepository characterRepository, IItemRepository itemRepository, IDomainEventPublisher eventPublisher)
        {
            this.characterRepository = characterRepository;
            this.itemRepository = itemRepository;
            this.eventPublisher = eventPublisher;
        }

        /// <summary>
        /// Execute item pickup.
        /// </summary>
        /// <param name="characterId">Character ID picking up the item</param>
        /// <param name="itemId">Item to pick up</param>
        /// <param name="quantity">Quantity to pick up</param>
        /// <returns>True if all items were added, false if inventory was full</returns>
        public bool Execute(CharacterId characterId, ItemId itemId, int quantity)
        {
            var character = characterRepository.Get(characterId);
            var itemDefinition = itemRepository.Get(itemId);

            var addedCount = character.Inventory.AddItem(itemDefinition, quantity);

            if (addedCount > 0)
            {
                eventPublisher.Publish(new ItemAcquiredEvent(characterId, itemId, addedCount));
            }

            return addedCount == quantity;
        }
    }
}