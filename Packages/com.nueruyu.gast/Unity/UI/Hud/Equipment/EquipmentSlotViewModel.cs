using System;
using Gast.Domain.Economy;
using Gast.Domain.Equipment;
using Gast.Unity.Infrastructure.Equipment;
using R3;

namespace Gast.Unity.UI.Hud.Equipment
{
    public class EquipmentSlotViewModel
    {
        readonly Action<EquipmentSlotId> unequipAction;

        public EquipmentSlotDefinition Definition { get; }
        public ReadOnlyReactiveProperty<ItemId?> Item { get; }

        public EquipmentSlotViewModel(
            EquipmentSlotDefinition definition,
            ReadOnlyReactiveProperty<ItemId?> item,
            Action<EquipmentSlotId> unequipAction)
        {
            Definition = definition;
            Item = item;
            this.unequipAction = unequipAction;
        }

        public void RequestUnequip() => unequipAction(Definition.Id);
    }
}
