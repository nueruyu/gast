using System.Collections.Generic;
using Gast.Core.Observables;
using Gast.Domain.Characters;
using Gast.Domain.Economy;

namespace Gast.Domain.Equipment
{
    public interface IEquipmentHost : ICharacterFacet
    {
        IEnumerable<EquipmentSlotId> Slots { get; }
        ILive<ItemId?> GetSlot(EquipmentSlotId slotId);
        void Equip(EquipmentSlotId slotId, ItemId itemId);
        void Unequip(EquipmentSlotId slotId);
    }
}
