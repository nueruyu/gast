using Gast.Domain.Characters;
using Gast.Domain.Economy;
using R3;

namespace Gast.Domain.Equipment
{
    public interface IEquipmentHost : ICharacterFacet
    {
        ReadOnlyReactiveProperty<ItemId?> Head { get; }
        ReadOnlyReactiveProperty<ItemId?> Body { get; }
        ReadOnlyReactiveProperty<ItemId?> Weapon { get; }

        void Equip(EquipmentSlot slot, ItemId itemId);
        void Unequip(EquipmentSlot slot);
    }
}
