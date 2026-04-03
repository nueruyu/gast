using System.Collections.Generic;
using Gast.Core.Observables;
using Gast.Domain.Economy;
using Gast.Domain.Equipment;
using Gast.Unity.Infrastructure.Equipment;

namespace Cryst.Infrastructure.Characters
{
    class EquipmentHost : IEquipmentHost
    {
        readonly List<EquipmentSlotId> slotOrder = new();
        readonly Dictionary<EquipmentSlotId, Live<ItemId?>> slots = new();

        public EquipmentHost(IEnumerable<EquipmentSlotDefinition> slotDefinitions)
        {
            foreach (var def in slotDefinitions)
            {
                slotOrder.Add(def.Id);
                slots[def.Id] = new Live<ItemId?>(null);
            }
        }

        public IEnumerable<EquipmentSlotId> Slots => slotOrder;

        public ILive<ItemId?> GetSlot(EquipmentSlotId slotId) => slots[slotId];

        public void Equip(EquipmentSlotId slotId, ItemId itemId)
        {
            if (slots.TryGetValue(slotId, out var live))
                live.Value = itemId;
        }

        public void Unequip(EquipmentSlotId slotId)
        {
            if (slots.TryGetValue(slotId, out var live))
                live.Value = null;
        }
    }
}
