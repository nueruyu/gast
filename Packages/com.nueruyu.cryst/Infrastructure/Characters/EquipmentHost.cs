using Gast.Core.Observables;
using Gast.Domain.Economy;
using Gast.Domain.Equipment;

namespace Cryst.Infrastructure.Characters
{
    class EquipmentHost : IEquipmentHost
    {
        readonly Live<ItemId?> head = new(null);
        readonly Live<ItemId?> body = new(null);
        readonly Live<ItemId?> weapon = new(null);

        public ILive<ItemId?> Head => head;
        public ILive<ItemId?> Body => body;
        public ILive<ItemId?> Weapon => weapon;

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
