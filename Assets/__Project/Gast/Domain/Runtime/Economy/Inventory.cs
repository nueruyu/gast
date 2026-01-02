using System;
using System.Collections.Generic;
using Gast.Core.Observables;

namespace Gast.Domain.Economy
{
    /// <summary>
    /// Manages item possession state.
    /// </summary>
    public class Inventory
    {
        readonly List<ItemStack> items = new();
        readonly Signal inventoryChanged = new();
        readonly int capacity;

        /// <summary>
        /// Signal fired when inventory contents change.
        /// </summary>
        public ISignal InventoryChanged => inventoryChanged;

        /// <summary>
        /// Current item list (read-only).
        /// </summary>
        public IReadOnlyList<ItemStack> Items => items;

        /// <summary>
        /// Maximum number of inventory slots.
        /// </summary>
        public int Capacity => capacity;

        public Inventory(int capacity)
        {
            this.capacity = capacity;
        }

        /// <summary>
        /// Add items to inventory.
        /// Stackable items are added to existing slots up to MaxStack, overflow creates new slots.
        /// </summary>
        /// <param name="itemDefinition">Item to add</param>
        /// <param name="quantity">Quantity to add</param>
        /// <returns>Actual quantity added (may be less than requested if inventory is full)</returns>
        public int AddItem(IItemDefinition itemDefinition, int quantity)
        {
            if (itemDefinition is null)
                throw new ArgumentNullException(nameof(itemDefinition));
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity));

            var remaining = quantity;

            // 1. Add to existing stacks (fill up to MaxStack)
            if (itemDefinition.MaxStack > 1)
            {
                foreach (var stack in items)
                {
                    if (stack.ItemId == itemDefinition.Id &&
                        stack.Quantity < itemDefinition.MaxStack)
                    {
                        var space = itemDefinition.MaxStack - stack.Quantity;
                        var add = Math.Min(remaining, space);

                        stack.Quantity += add;
                        remaining -= add;

                        if (remaining <= 0)
                            break;
                    }
                }
            }

            // 2. Add to new slots
            while (remaining > 0)
            {
                if (items.Count >= capacity)
                    break;

                var add = Math.Min(remaining, itemDefinition.MaxStack);
                items.Add(new ItemStack(itemDefinition.Id, add));
                remaining -= add;
            }

            var addedAmount = quantity - remaining;

            if (addedAmount > 0)
            {
                inventoryChanged.Publish();
            }

            return addedAmount;
        }

        /// <summary>
        /// Remove specified quantity of an item.
        /// </summary>
        /// <returns>True if removed, false if insufficient quantity (nothing removed in this case)</returns>
        public bool RemoveItem(ItemId itemId, int quantity)
        {
            if (!HasItem(itemId, quantity))
                return false;

            var remainingToRemove = quantity;

            for (var i = items.Count - 1; i >= 0; i--)
            {
                var stack = items[i];
                if (stack.ItemId == itemId)
                {
                    var take = Math.Min(stack.Quantity, remainingToRemove);
                    stack.Quantity -= take;
                    remainingToRemove -= take;

                    if (stack.Quantity <= 0)
                    {
                        items.RemoveAt(i);
                    }

                    if (remainingToRemove <= 0)
                        break;
                }
            }

            inventoryChanged.Publish();
            return true;
        }

        /// <summary>
        /// Check if inventory has at least the specified quantity of an item.
        /// </summary>
        public bool HasItem(ItemId itemId, int quantity = 1)
        {
            var total = 0L;
            foreach (var stack in items)
            {
                if (stack.ItemId == itemId)
                {
                    total += stack.Quantity;
                }
            }
            return total >= quantity;
        }

        /// <summary>
        /// Remove all items from inventory.
        /// </summary>
        public void Clear()
        {
            items.Clear();
            inventoryChanged.Publish();
        }
    }
}