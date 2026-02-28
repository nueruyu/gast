using System;
using Gast.Unity.Shared.UnityExtensions;
using Gast.Unity.UI.Hud.AIStatus;
using Gast.Unity.UI.Hud.Inventory;
using Gast.Unity.UI.Hud.Objectives;
using R3;
using UnityEngine.UIElements;

namespace Gast.Unity.UI.Hud
{
    public class GameHudView : VisualElement
    {
        public GameHudView(
            VisualTreeAsset asset,
            VisualElement playerStatusView,
            InventoryView inventoryView,
            AIStatusView aiStatusView,
            AIObjectivesView aiObjectivesView)
        {
            asset.CloneTree(this);
            focusable = true;
            tabIndex = -1;

            this.Q("PlayerStatusContainer").Add(playerStatusView);
            this.Q("InventoryContainer").Add(inventoryView);
            this.Q("AIStatusContainer").Add(aiStatusView);
            this.Q("AIObjectivesContainer").Add(aiObjectivesView);
        }

        public IDisposable Bind(GameHudViewModel viewModel)
        {
            var disposables = new CompositeDisposable();

            viewModel.IsVisible
                .Subscribe(visible => style.display = visible ? DisplayStyle.Flex : DisplayStyle.None)
                .AddTo(disposables);

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
    }
}
