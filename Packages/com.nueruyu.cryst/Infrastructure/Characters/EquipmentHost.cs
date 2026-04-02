using Gast.Domain.Economy;
using Gast.Domain.Equipment;
using R3;

namespace Cryst.Infrastructure.Characters
{
    class EquipmentHost : IEquipmentHost
    {
        readonly ReactiveProperty<ItemId?> head = new(null);
        readonly ReactiveProperty<ItemId?> body = new(null);
        readonly ReactiveProperty<ItemId?> weapon = new(null);

        public ReadOnlyReactiveProperty<ItemId?> Head => head;
        public ReadOnlyReactiveProperty<ItemId?> Body => body;
        public ReadOnlyReactiveProperty<ItemId?> Weapon => weapon;

        public void Equip(EquipmentSlot slot, ItemId itemId)
        {
            switch (slot)
            {
                case EquipmentSlot.Head: head.Value = itemId; break;
                case EquipmentSlot.Body: body.Value = itemId; break;
                case EquipmentSlot.Weapon: weapon.Value = itemId; break;
            }
        }

        public void Unequip(EquipmentSlot slot)
        {
            switch (slot)
            {
                case EquipmentSlot.Head: head.Value = null; break;
                case EquipmentSlot.Body: body.Value = null; break;
                case EquipmentSlot.Weapon: weapon.Value = null; break;
            }
        }
    }
}
