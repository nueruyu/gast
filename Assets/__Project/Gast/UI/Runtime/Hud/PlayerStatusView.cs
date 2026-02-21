// Assets/__Project/Gast/UI/Runtime/Hud/PlayerStatusView.cs
using System;
using Gast.Shared.UnityExtensions;
using R3;
using UnityEngine.UIElements;

namespace Gast.UI.Hud
{
    public class PlayerStatusView : VisualElement
    {
        readonly Label moneyLabel;
        readonly GaugeView hpGauge;

        public PlayerStatusView(VisualTreeAsset asset, UIAssetSettings assetSettings)
        {
            asset.CloneTree(this);
            moneyLabel = this.Q<Label>("MoneyLabel");

            hpGauge = new GaugeView(assetSettings.GaugeView);
            this.Q("HpContainer").Add(hpGauge);
        }

        public IDisposable Bind(PlayerStatusViewModel viewModel)
        {
            var d = new CompositeDisposable();

            hpGauge.Bind(viewModel.HpRatio, viewModel.HpText).AddTo(d);

            viewModel.CurrentMoney
                .Subscribe(amount => moneyLabel.text = $"{amount:N0} G")
                .AddTo(d);

            return d;
        }
    }
}
