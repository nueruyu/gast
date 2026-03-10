using Gast.Domain.Economy;
using Gast.Domain.Items;
using UnityEngine;

namespace Gast.Unity.Infrastructure.Items
{
    public class PlacedItemFactory : IPlacedItemFactory
    {
        readonly IItemRepository itemRepository;

        public PlacedItemFactory(IItemRepository itemRepository)
        {
            this.itemRepository = itemRepository;
        }

        public GameObject CreatePreview(ItemId itemId)
        {
            var itemDefinition = (ItemDefinition)itemRepository.Get(itemId);

            if (itemDefinition.PlacementPrefab == null)
            {
                return null;
            }

            var preview = Object.Instantiate(itemDefinition.PlacementPrefab);

            // Disable colliders on preview so they don't interfere with physics checks
            foreach (var collider in preview.GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }

            return preview;
        }

        public bool Create(ItemId itemId, Vector3 position, Quaternion rotation)
        {
            var itemDefinition = (ItemDefinition)itemRepository.Get(itemId);

            if (itemDefinition.PlacementPrefab == null)
            {
                Debug.LogWarning($"[PlacedItemFactory] Item '{itemDefinition.Name}' has no placement prefab assigned.");
                return false;
            }

            Object.Instantiate(itemDefinition.PlacementPrefab, position, rotation);
            return true;
        }
    }
}
