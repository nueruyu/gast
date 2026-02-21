// Assets/__Project/Gast/UI/Runtime/Hud/InventoryView.cs
using System;
using R3;
using UnityEngine.UIElements;

namespace Gast.UI.Hud
{
    public class InventoryView : VisualElement
    {
        private const string ItemSlotUssClassName = "inventory__item-slot";
        private const string ItemTextUssClassName = "inventory__item-text";

        public InventoryView(VisualTreeAsset asset)
        {
            asset.CloneTree(this);
        }

        public IDisposable Bind(InventoryViewModel viewModel)
        {
            return viewModel.InventoryItems.Subscribe(items =>
            {
                Clear();
                foreach (var stack in items)
                {
                    CreateItemSlot(stack);
                }
            });
        }

        private void CreateItemSlot(ItemStackViewModel stackViewModel)
        {
            var slot = new VisualElement();
            slot.AddToClassList(ItemSlotUssClassName);

            var text = new Label($"{stackViewModel.ItemDefinition.Name} x{stackViewModel.ItemStack.Quantity}");
            text.AddToClassList(ItemTextUssClassName);

            slot.Add(text);
            Add(slot);
        }
    }
}
