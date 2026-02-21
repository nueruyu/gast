using System;
using Gast.UI.Controls;
using R3;
using UnityEngine.UIElements;

namespace Cryst.UI.Hud.Status
{
    public class PlayerStatusView : VisualElement
    {
        readonly Label moneyLabel;
        readonly GaugeView hpGauge;

        public PlayerStatusView(VisualTreeAsset asset)
        {
            asset.CloneTree(this);
            moneyLabel = this.Q<Label>("MoneyLabel");

            hpGauge = new GaugeView();
            this.Q("HpContainer").Add(hpGauge);
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
