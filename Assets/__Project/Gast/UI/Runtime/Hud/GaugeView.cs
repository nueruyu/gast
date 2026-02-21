using System;
using R3;
using UnityEngine.UIElements;

namespace Gast.UI.Hud
{
    public class GaugeView : VisualElement
    {
        private readonly VisualElement fillElement;
        private readonly Label labelElement;

        public GaugeView(VisualTreeAsset asset)
        {
            asset.CloneTree(this);
            fillElement = this.Q<VisualElement>("Fill");
            labelElement = this.Q<Label>("Label");
        }

        public IDisposable Bind(ReadOnlyReactiveProperty<float> ratio, ReadOnlyReactiveProperty<string> text)
        {
            var d = new CompositeDisposable();

            ratio
                .Subscribe(r => fillElement.style.width = Length.Percent(r * 100f))
                .AddTo(d);

            text
                .Subscribe(t => labelElement.text = t)
                .AddTo(d);

            return d;
        }
    }
}