using Gast.Domain.Economy;
using UnityEngine;

namespace Gast.Domain.Items
{
    public interface IPlacedItemFactory
    {
        /// <summary>
        /// Creates a preview instance of the placed item for positioning.
        /// </summary>
        /// <returns>The preview GameObject, or null if the item has no placement prefab.</returns>
        GameObject CreatePreview(ItemId itemId);

        /// <summary>
        /// Creates a placed item in the world.
        /// </summary>
        /// <returns>True if the item was successfully placed, otherwise false.</returns>
        bool Create(ItemId itemId, Vector3 position, Quaternion rotation);
    }
}
