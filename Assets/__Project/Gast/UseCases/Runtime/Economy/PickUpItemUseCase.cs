using DescrioGames.Domain.Characters;
using DescrioGames.Domain.Economy;

namespace DescrioGames.UseCases.Economy
{
    /// <summary>
    /// Use case for picking up items and adding them to inventory.
    /// </summary>
    public class PickUpItemUseCase
    {
        readonly ICharacterRepository characterRepository;
        readonly IItemRepository itemRepository;

        public PickUpItemUseCase(ICharacterRepository characterRepository, IItemRepository itemRepository)
        {
            this.characterRepository = characterRepository;
            this.itemRepository = itemRepository;
        }

        /// <summary>
        /// Execute item pickup.
        /// </summary>
        /// <param name="characterId">Character ID picking up the item</param>
        /// <param name="itemDefinition">Item to pick up</param>
        /// <param name="quantity">Quantity to pick up</param>
        /// <returns>True if all items were added, false if inventory was full</returns>
        public bool Execute(CharacterId characterId, ItemId itemId, int quantity)
        {
            var character = characterRepository.Get(characterId);
            var itemDefinition = itemRepository.Get(itemId);

            var addedCount = character.Inventory.AddItem(itemDefinition, quantity);

            return addedCount == quantity;
        }
    }
}