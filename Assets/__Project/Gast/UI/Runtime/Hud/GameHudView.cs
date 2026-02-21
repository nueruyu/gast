using System;
using Gast.Shared.UnityExtensions;
using R3;
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
        const string AiStatusGroupName = "AiStatusGroup";
        const string StopAiButtonName = "StopAiButton";
        const string AiObjectivesGroupName = "AiObjectivesGroup";
        const string AiObjectivesListName = "AiObjectivesList";
        const string MinimizeButtonName = "MinimizeButton";
        const string AiObjectivesScrollViewName = "AiObjectivesScrollView";

        const string ItemSlotUssClassName = "game-hud__item-slot";
        const string ItemTextUssClassName = "game-hud__item-text";

        readonly VisualElement hpFill;
        readonly Label hpLabel;
        readonly Label moneyLabel;
        readonly VisualElement inventoryContainer;
        readonly VisualElement aiStatusGroup;
        readonly Button stopAiButton;
        readonly VisualElement aiObjectivesGroup;
        readonly VisualElement aiObjectivesList;
        readonly Button minimizeButton;
        readonly ScrollView aiObjectivesScrollView;
        readonly UIAssetSettings assetSettings;

        /// <summary>
        /// Creates the HUD view from a UXML asset.
        /// </summary>
        public GameHudView(VisualTreeAsset asset, UIAssetSettings assetSettings)
        {
            this.assetSettings = assetSettings;
            asset.CloneTree(this);

            focusable = true;
            tabIndex = -1; // Prevent focusing via keyboard navigation

            hpFill = this.Q<VisualElement>(HpFillName);
            hpLabel = this.Q<Label>(HpLabelName);
            moneyLabel = this.Q<Label>(MoneyLabelName);
            inventoryContainer = this.Q<VisualElement>(InventoryContainerName);

            // AI elements
            aiStatusGroup = this.Q<VisualElement>(AiStatusGroupName);
            stopAiButton = this.Q<Button>(StopAiButtonName);

            aiObjectivesGroup = this.Q<VisualElement>(AiObjectivesGroupName);
            aiObjectivesList = this.Q<VisualElement>(AiObjectivesListName);
            minimizeButton = this.Q<Button>(MinimizeButtonName);
            aiObjectivesScrollView = this.Q<ScrollView>(AiObjectivesScrollViewName);
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

            // Bind AI Status visibility
            viewModel.IsAiControlActive
                .Subscribe(active => aiStatusGroup.style.display = active ? DisplayStyle.Flex : DisplayStyle.None)
                .AddTo(disposables);

            // Bind Stop Button
            stopAiButton.SubscribeEvent<ClickEvent>(_ => viewModel.StopAiControl())
                .AddTo(disposables);

            // Bind AI Objectives visibility
            viewModel.IsAiControlActive
                .Subscribe(active => aiObjectivesGroup.style.display = active ? DisplayStyle.Flex : DisplayStyle.None)
                .AddTo(disposables);

            // Bind AI Objectives List
            viewModel.AiObjectives
                .Subscribe(objectives =>
                {
                    aiObjectivesList.Clear();
                    foreach (var objectiveVm in objectives)
                    {
                        var objectiveView = new AIObjectiveView(assetSettings.AIObjectiveView);
                        objectiveView.Bind(objectiveVm);
                        aiObjectivesList.Add(objectiveView);
                    }
                })
                .AddTo(disposables);

            // Minimize Button Logic
            bool isMinimized = false;
            minimizeButton.SubscribeEvent<ClickEvent>(_ =>
            {
                isMinimized = !isMinimized;
                aiObjectivesScrollView.style.display = isMinimized ? DisplayStyle.None : DisplayStyle.Flex;
                minimizeButton.text = isMinimized ? "+" : "-";
            }).AddTo(disposables);

            this.SubscribeEvent<FocusInEvent>(evt =>
            {
                viewModel.SetFocus(true);
            }).AddTo(disposables);

            this.SubscribeEvent<FocusOutEvent>(evt =>
            {
                if (evt.relatedTarget is VisualElement target &&
                    !Contains(target))
                {
                    viewModel.SetFocus(false);
                }
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
