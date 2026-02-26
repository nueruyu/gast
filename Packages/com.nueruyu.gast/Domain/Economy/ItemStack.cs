namespace Gast.Domain.Economy
{
    /// <summary>
    /// Item slot in inventory. Pairs an item with its quantity.
    /// </summary>
    public class ItemStack
    {
        public ItemId ItemId { get; }
        public int Quantity { get; set; }

        public ItemStack(ItemId itemId, int quantity)
        {
            ItemId = itemId;
            Quantity = quantity;
        }
    }
}