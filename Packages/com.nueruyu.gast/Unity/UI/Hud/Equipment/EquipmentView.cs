using System;
using Gast.Domain.Economy;
using Gast.Domain.Equipment;
using R3;
using UnityEngine.UIElements;

namespace Gast.Unity.UI.Hud.Equipment
{
    public class EquipmentView : VisualElement
    {
        const string SlotFilledUssClassName = "equipment__slot--filled";
        const string SlotLabelUssClassName = "equipment__slot-label";

        VisualElement headSlot;
        VisualElement bodySlot;
        VisualElement weaponSlot;

        EquipmentViewModel viewModel;

        public EquipmentView(VisualTreeAsset asset)
        {
            asset.CloneTree(this);

            headSlot = this.Q<VisualElement>("head-slot");
            bodySlot = this.Q<VisualElement>("body-slot");
            weaponSlot = this.Q<VisualElement>("weapon-slot");

            headSlot.RegisterCallback<PointerDownEvent>(_ => viewModel?.RequestUnequip(EquipmentSlot.Head));
            bodySlot.RegisterCallback<PointerDownEvent>(_ => viewModel?.RequestUnequip(EquipmentSlot.Body));
            weaponSlot.RegisterCallback<PointerDownEvent>(_ => viewModel?.RequestUnequip(EquipmentSlot.Weapon));
        }

        public IDisposable Bind(EquipmentViewModel vm)
        {
            viewModel = vm;
            var disposables = new CompositeDisposable();

            vm.HeadItem.Subscribe(itemId => UpdateSlotUI(headSlot, itemId)).AddTo(disposables);
            vm.BodyItem.Subscribe(itemId => UpdateSlotUI(bodySlot, itemId)).AddTo(disposables);
            vm.WeaponItem.Subscribe(itemId => UpdateSlotUI(weaponSlot, itemId)).AddTo(disposables);

            return disposables;
        }

        void UpdateSlotUI(VisualElement slot, ItemId? itemId)
        {
            var label = slot.Q<Label>(className: SlotLabelUssClassName);
            if (label == null)
            {
                label = new Label();
                label.AddToClassList(SlotLabelUssClassName);
                slot.Add(label);
            }

            if (itemId.HasValue)
            {
                slot.AddToClassList(SlotFilledUssClassName);
                label.text = itemId.Value.Value;
            }
            else
            {
                slot.RemoveFromClassList(SlotFilledUssClassName);
                label.text = "Empty";
            }
        }
    }
}
