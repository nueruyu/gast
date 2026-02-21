using UnityEngine.UIElements;

namespace Gast.UI.Controls
{
    [UxmlElement]
    public partial class Gauge : VisualElement
    {
        static readonly string UssClassName = "gauge";
        static readonly string ProgressBarUssClassName = UssClassName + "__progress-bar";
        static readonly string LabelUssClassName = UssClassName + "__label";

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

        public Gauge()
        {
            AddToClassList(UssClassName);

            progressBar = new VisualElement();
            progressBar.AddToClassList(ProgressBarUssClassName);
            Add(progressBar);

            label = new Label();
            label.AddToClassList(LabelUssClassName);
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