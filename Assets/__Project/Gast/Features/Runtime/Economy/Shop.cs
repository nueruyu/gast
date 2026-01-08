using Cysharp.Threading.Tasks;
using Gast.Api.Economy;
using Gast.Core.Commands;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Features.Interactions;
using System;
using UnityEngine;

namespace Gast.Features.Economy
{
    public class Shop : MonoBehaviour
    {
        public void Initialize(
            ICommandDispatcher commandDispatcher,
            IItemRepository itemRepository,
            InteractionSystem interactionSystem)
        {
            if (commandDispatcher is null)
                throw new ArgumentNullException(nameof(commandDispatcher));
            if (itemRepository is null)
                throw new ArgumentNullException(nameof(itemRepository));
            if (interactionSystem is null)
                throw new ArgumentNullException(nameof(interactionSystem));

            var shopItems = GetComponentsInChildren<ShopItem>();

            foreach (var shopItem in shopItems)
            {
                var itemDefinition = itemRepository.Get(shopItem.ItemId);

                shopItem.Initialize(
                    interactionSystem,
                    $"Buy {itemDefinition.Name} ({itemDefinition.Price}G)");

                shopItem.Buy
                    .Subscribe(buyerId => OnBuyItem(buyerId, itemDefinition))
                    .AddTo(shopItem.destroyCancellationToken);
            }

            Debug.Log($"ShopManager initialized {shopItems.Length} shop items");

            void OnBuyItem(CharacterId buyerId, IItemDefinition itemDefinition)
            {
                var success = commandDispatcher.Dispatch<BuyItemCommand, bool>(new(
                    buyerId,
                    itemDefinition.Id));

                if (success)
                {
                    Debug.Log($"Purchased {itemDefinition.Name} for {itemDefinition.Price}G");
                }
                else
                {
                    Debug.LogWarning($"Failed to purchase {itemDefinition.Name} - insufficient funds or inventory full");
                }
            }
        }
    }
}