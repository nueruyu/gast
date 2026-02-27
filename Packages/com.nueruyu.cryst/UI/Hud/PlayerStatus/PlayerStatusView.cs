using System;
using R3;
using UnityEngine.UIElements;
using Gauge = Gast.Unity.UI.Controls.Gauge;

namespace Cryst.UI.Hud.PlayerStatus
{
    public class PlayerStatusView : VisualElement
    {
        readonly Label moneyLabel;
        readonly Gauge hpGauge;

        public PlayerStatusView(VisualTreeAsset asset)
        {
            asset.CloneTree(this);

            hpGauge = this.Q<Gauge>("HpGauge");
            moneyLabel = this.Q<Label>("MoneyLabel");
        }

        public IDisposable Bind(PlayerStatusViewModel viewModel)
        {
            var d = new CompositeDisposable();

            viewModel.HpRatio
                .Subscribe(ratio => hpGauge.Value = ratio * hpGauge.MaxValue)
                .AddTo(d);

            viewModel.HpText
                .Subscribe(text => hpGauge.LabelText = text)
                .AddTo(d);

            viewModel.CurrentMoney
                .Subscribe(amount => moneyLabel.text = $"{amount:N0} G")
                .AddTo(d);

            return d;
        }
    }
}