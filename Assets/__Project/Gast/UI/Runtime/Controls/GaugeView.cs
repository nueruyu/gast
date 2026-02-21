using UnityEngine.UIElements;

namespace Gast.UI.Controls
{
    [UxmlElement]
    public partial class GaugeView : VisualElement
    {
        readonly VisualElement progressBar;
        readonly Label label;

        float currentValue;
        float maxValue = 100f;

        [UxmlAttribute]
        public float Value
        {
            get => currentValue;
            set
            {
                currentValue = value;
                UpdateView();
            }
        }

        [UxmlAttribute]
        public float MaxValue
        {
            get => maxValue;
            set
            {
                maxValue = value;
                UpdateView();
            }
        }

        [UxmlAttribute]
        public string LabelText
        {
            get => label.text;
            set => label.text = value;
        }

        public GaugeView()
        {
            AddToClassList("gauge-view");

            progressBar = new VisualElement();
            progressBar.AddToClassList("gauge-view__progress-bar");
            Add(progressBar);

            label = new Label();
            label.AddToClassList("gauge-view__label");
            Add(label);

            Value = 100f;
        }

        void UpdateView()
        {
            var progress = (maxValue > 0) ? (currentValue / maxValue) * 100f : 0f;
            progressBar.style.width = new Length(progress, LengthUnit.Percent);
        }
    }
}