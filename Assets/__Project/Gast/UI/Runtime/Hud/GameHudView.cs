using System;
using Gast.Shared.UnityExtensions;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gast.UI.Hud
{
    /// <summary>
    /// Visual representation of the game HUD.
    /// </summary>
    public class GameHudView : VisualElement
    {
        const string HpFillName = "HpFill";
        const string HpLabelName = "HpLabel";
        const string MoneyLabelName = "MoneyLabel";
        const string InventoryContainerName = "InventoryContainer";
        const string ItemSlotUssClassName = "game-hud__item-slot";
        const string ItemTextUssClassName = "game-hud__item-text";

        readonly VisualElement hpFill;
        readonly Label hpLabel;
        readonly Label moneyLabel;
        readonly VisualElement inventoryContainer;

        /// <summary>
        /// Creates the HUD view from a UXML asset.
        /// </summary>
        public GameHudView(VisualTreeAsset asset)
        {
            asset.CloneTree(this);

            focusable = true;
            //tabIndex = -1; // Prevent focusing via keyboard navigation

            hpFill = this.Q<VisualElement>(HpFillName);
            hpLabel = this.Q<Label>(HpLabelName);
            moneyLabel = this.Q<Label>(MoneyLabelName);
            inventoryContainer = this.Q<VisualElement>(InventoryContainerName);
        }

        /// <summary>
        /// Binds the view to a ViewModel and returns a disposable subscription.
        /// </summary>
        /// <param name="viewModel">The ViewModel to bind to.</param>
        /// <returns>A disposable that cleans up the binding when disposed.</returns>
        public IDisposable Bind(GameHudViewModel viewModel)
        {
            var disposables = new CompositeDisposable();

            // Visibility
            viewModel.IsVisible
                .Subscribe(visible => style.display = visible ? DisplayStyle.Flex : DisplayStyle.None)
                .AddTo(disposables);

            // HP Ratio
            viewModel.HpRatio
                .Subscribe(ratio => hpFill.style.width = Length.Percent(ratio * 100f))
                .AddTo(disposables);

            // HP Text
            viewModel.HpText
                .Subscribe(text => hpLabel.text = text)
                .AddTo(disposables);

            // Money (format with thousand separators and "G" suffix)
            viewModel.CurrentMoney
                .Subscribe(amount => moneyLabel.text = $"{amount:N0} G")
                .AddTo(disposables);

            // Inventory List
            viewModel.InventoryItems
                .Subscribe(items =>
                {
                    inventoryContainer.Clear();

                    foreach (var stack in items)
                    {
                        CreateItemSlot(stack);
                    }
                })
                .AddTo(disposables);

            this.SubscribeEvent<FocusInEvent>(evt =>
            {
                viewModel.SetFocus(true);
            }).AddTo(disposables);
            
            this.SubscribeEvent<FocusOutEvent>(evt =>
            {
                viewModel.SetFocus(false);
            }).AddTo(disposables);

            return disposables;
        }

        void CreateItemSlot(ItemStackViewModel stackViewModel)
        {
            var slot = new VisualElement();
            slot.AddToClassList(ItemSlotUssClassName);

            var text = new Label($"{stackViewModel.ItemDefinition.Name} x{stackViewModel.ItemStack.Quantity}");
            text.AddToClassList(ItemTextUssClassName);

            slot.Add(text);
            inventoryContainer.Add(slot);
        }
    }
}