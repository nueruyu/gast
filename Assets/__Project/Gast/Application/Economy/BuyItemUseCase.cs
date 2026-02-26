using Gast.Core.Commands;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using System;

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

            if (!character.Is(out IWalletHost walletHost) || !character.Is(out IInventoryHost inventoryHost))
            {
                throw new InvalidOperationException($"Character '{character.Id}' does not support economic actions.");
            }

            var wallet = walletHost.Wallet;
            var inventory = inventoryHost.Inventory;

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