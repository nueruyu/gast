using Gast.Domain.Economy;
using UnityEngine;

namespace Gast.Application.Economy
{
    /// <summary>
    /// Provides a way to retrieve game assets related to items,
    /// abstracting the underlying storage mechanism.
    /// </summary>
    public interface IItemAssetService
    {
        /// <summary>
        /// Gets the icon sprite for a given item ID.
        /// </summary>
        /// <param name="itemId">The ID of the item.</param>
        /// <returns>The corresponding sprite, or null if not found.</returns>
        Sprite GetItemIcon(ItemId itemId);
    }
}
