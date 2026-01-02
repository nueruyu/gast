using DescrioGames.Domain.Characters;
using DescrioGames.Domain.Economy;

namespace DescrioGames.UseCases.Economy
{
    /// <summary>
    /// Use case for purchasing items by spending currency.
    /// </summary>
    public class BuyItemUseCase
    {
        readonly ICharacterRepository characterRepository;
        readonly IItemRepository itemRepository;

        public BuyItemUseCase(ICharacterRepository characterRepository, IItemRepository itemRepository)
        {
            this.characterRepository = characterRepository;
            this.itemRepository = itemRepository;
        }

        /// <summary>
        /// Execute item purchase.
        /// </summary>
        /// <param name="buyerId">Character ID of the buyer</param>
        /// <param name="itemDefinition">Item to purchase</param>
        /// <returns>True if purchase successful, false otherwise</returns>
        public bool Execute(CharacterId buyerId, ItemId itemId)
        {
            var character = characterRepository.Get(buyerId);
            var itemDefinition = itemRepository.Get(itemId);

            var wallet = character.Wallet;
            var inventory = character.Inventory;

            if (!wallet.TrySpend(itemDefinition.Price))
            {
                return false;
            }

            var addedCount = inventory.AddItem(itemDefinition, 1);

            if (addedCount == 0)
            {
                wallet.Add(itemDefinition.Price);
                return false;
            }

            return true;
        }
    }
}