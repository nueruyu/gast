using Gast.Core.Observables;
using Gast.Domain.Characters;
using Gast.Domain.Economy;

namespace Gast.Domain.Equipment
{
    public interface IEquipmentHost : ICharacterFacet
    {
        ILive<ItemId?> Head { get; }
        ILive<ItemId?> Body { get; }
        ILive<ItemId?> Weapon { get; }

        void Equip(EquipmentSlot slot, ItemId itemId);
        void Unequip(EquipmentSlot slot);
    }
}
