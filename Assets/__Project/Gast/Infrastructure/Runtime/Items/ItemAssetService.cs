using Gast.Application.Economy;
using Gast.Domain.Economy;
using UnityEngine;

namespace Gast.Infrastructure.Items
{
    /// <summary>
    /// Infrastructure-level implementation that retrieves item assets
    /// from ScriptableObject definitions.
    /// </summary>
    public class ItemAssetService : IItemAssetService
    {
        readonly IItemRepository itemRepository;

        public ItemAssetService(IItemRepository itemRepository)
        {
            this.itemRepository = itemRepository;
        }

        public Sprite GetItemIcon(ItemId itemId)
        {
            var definition = (ItemDefinition)itemRepository.Get(itemId);
            return definition?.Icon;
        }
    }
}