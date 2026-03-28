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
        readonly Gauge hungerGauge;

        public PlayerStatusView(VisualTreeAsset asset)
        {
            asset.CloneTree(this);

            hpGauge = this.Q<Gauge>("HpGauge");
            hungerGauge = this.Q<Gauge>("HungerGauge");
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

            viewModel.HungerRatio
                .Subscribe(ratio => hungerGauge.Value = ratio * hungerGauge.MaxValue)
                .AddTo(d);

            viewModel.HungerText
                .Subscribe(text => hungerGauge.LabelText = text)
                .AddTo(d);

            viewModel.CurrentMoney
                .Subscribe(amount => moneyLabel.text = $"{amount:N0} G")
                .AddTo(d);

            return d;
        }
    }
}