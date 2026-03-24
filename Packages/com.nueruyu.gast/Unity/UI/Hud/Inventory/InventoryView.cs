using System;
using R3;
using UnityEngine.UIElements;

namespace Gast.Unity.UI.Hud.Inventory
{
    public class InventoryView : VisualElement
    {
        const int SlotCount = 10;
        const string ItemSlotUssClassName = "inventory__item-slot";
        const string SlotNumberUssClassName = "inventory__slot-number";
        const string ItemTextUssClassName = "inventory__item-text";

        public InventoryView(VisualTreeAsset asset)
        {
            asset.CloneTree(this);
        }

        public IDisposable Bind(InventoryViewModel viewModel)
        {
            return viewModel.InventoryItems.Subscribe(items =>
            {
                Clear();
                for (var i = 0; i < SlotCount; i++)
                {
                    var stack = i < items.Count ? items[i] : null;
                    CreateItemSlot(stack, i);
                }
            });
        }

        void CreateItemSlot(ItemStackViewModel stackViewModel, int slotIndex)
        {
            var slot = new VisualElement();
            slot.AddToClassList(ItemSlotUssClassName);

            var slotNumber = (slotIndex + 1) % 10;
            var numberLabel = new Label(slotNumber.ToString());
            numberLabel.AddToClassList(SlotNumberUssClassName);
            slot.Add(numberLabel);

            if (stackViewModel != null)
            {
                var text = new Label(
                    $"{stackViewModel.ItemDefinition.Name}\nx{stackViewModel.ItemStack.Quantity}");
                text.AddToClassList(ItemTextUssClassName);
                slot.Add(text);
            }

            Add(slot);
        }
    }
}