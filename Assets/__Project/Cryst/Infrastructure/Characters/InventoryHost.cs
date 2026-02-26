using Gast.Domain.Economy;

namespace Cryst.Infrastructure.Characters
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