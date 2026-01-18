using Gast.Core.Commands;
using Gast.Domain.Characters;
using Gast.Domain.Economy;

namespace Gast.Application.Economy
{
    /// <summary>
    /// Use case for purchasing items by spending currency.
    /// </summary>
    public class BuyItemUseCase : ICommandHandler<BuyItemCommand, bool>
    {
        readonly ICharacterRepository characterRepository;
        readonly IItemRepository itemRepository;

        public BuyItemUseCase(ICharacterRepository characterRepository, IItemRepository itemRepository)
        {
            this.characterRepository = characterRepository;
            this.itemRepository = itemRepository;
        }

        public bool Execute(in BuyItemCommand command)
        {
            var character = characterRepository.Get(command.BuyerId);
            var itemDefinition = itemRepository.Get(command.ItemId);

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