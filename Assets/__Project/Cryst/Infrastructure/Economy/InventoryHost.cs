using Gast.Domain.Economy;

namespace Cryst.Infrastructure.Economy
{
    class InventoryHost : IInventoryHost
    {
        public Inventory Inventory { get; }

        public InventoryHost(Inventory inventory)
        {
            Inventory = inventory;
        }
    }
}