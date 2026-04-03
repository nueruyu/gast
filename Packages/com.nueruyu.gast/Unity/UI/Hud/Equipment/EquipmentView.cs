using System;
using Gast.Domain.Economy;
using R3;
using UnityEngine.UIElements;

namespace Gast.Unity.UI.Hud.Equipment
{
    public class EquipmentView : VisualElement
    {
        const string SlotUssClassName = "equipment__slot";
        const string SlotFilledUssClassName = "equipment__slot--filled";
        const string SlotTitleUssClassName = "equipment__slot-title";
        const string SlotLabelUssClassName = "equipment__slot-label";

        public EquipmentView(VisualTreeAsset asset)
        {
            asset.CloneTree(this);
        }

        public IDisposable Bind(EquipmentViewModel viewModel)
        {
            var disposables = new CompositeDisposable();

            Clear();
            foreach (var slotVM in viewModel.SlotViewModels)
                Add(CreateSlotElement(slotVM, disposables));

            return disposables;
        }

        VisualElement CreateSlotElement(EquipmentSlotViewModel slotVM, CompositeDisposable disposables)
        {
            var slot = new VisualElement();
            slot.AddToClassList(SlotUssClassName);

            var title = new Label(slotVM.Definition.DisplayName);
            title.AddToClassList(SlotTitleUssClassName);
            slot.Add(title);

            var label = new Label();
            label.AddToClassList(SlotLabelUssClassName);
            slot.Add(label);

            slotVM.Item
                .Subscribe(itemId => UpdateSlot(slot, label, itemId))
                .AddTo(disposables);

            slot.RegisterCallback<PointerDownEvent>(_ => slotVM.RequestUnequip());

            return slot;
        }

        void UpdateSlot(VisualElement slot, Label label, ItemId? itemId)
        {
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
