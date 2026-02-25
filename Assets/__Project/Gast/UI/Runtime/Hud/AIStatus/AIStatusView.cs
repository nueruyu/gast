using System;
using Gast.Shared.UnityExtensions;
using R3;
using UnityEngine.UIElements;

namespace Gast.UI.Hud
{
    public class AIStatusView : VisualElement
    {
        private readonly Button stopAiButton;

        public AIStatusView(VisualTreeAsset asset)
        {
            asset.CloneTree(this);
            stopAiButton = this.Q<Button>("StopAiButton");
        }

        public IDisposable Bind(AIStatusViewModel viewModel)
        {
            var d = new CompositeDisposable();

            viewModel.IsAiControlActive
                .Subscribe(active => style.display = active ? DisplayStyle.Flex : DisplayStyle.None)
                .AddTo(d);

            stopAiButton.SubscribeEvent<ClickEvent>(_ => viewModel.StopAiControl())
                .AddTo(d);

            return d;
        }
    }
}
