using System;
using Gast.UI.Controls;
using R3;
using UnityEngine.UIElements;

namespace Gast.UI.Hud.Objectives
{
    public class AIObjectiveView : VisualElement
    {
        const string ItemIconName = "ItemIcon";
        const string DescriptionName = "Description";
        const string ProgressTextName = "ProgressText";
        const string CompletedUssClassName = "objective-item--completed";

        readonly VisualElement icon;
        readonly Label description;
        readonly Label progressText;
        readonly GaugeView progressGauge;

        public AIObjectiveView(VisualTreeAsset asset)
        {
            asset.CloneTree(this);
            icon = this.Q<VisualElement>(ItemIconName);
            description = this.Q<Label>(DescriptionName);
            progressText = this.Q<Label>(ProgressTextName);

            var progressContainer = this.Q<VisualElement>("ProgressBarContainer");
            if (progressContainer != null)
            {
                progressGauge = new GaugeView();
                progressGauge.LabelText = "";
                progressContainer.Add(progressGauge);
            }
        }

        public IDisposable Bind(IAIObjectiveViewModel viewModel)
        {
            var disposables = new CompositeDisposable();

            description.text = viewModel.Description;

            icon.style.backgroundImage = viewModel.ItemIcon != null
                ? new StyleBackground(viewModel.ItemIcon)
                : null;
            icon.style.display = viewModel.ItemIcon != null ? DisplayStyle.Flex : DisplayStyle.None;

            if (progressGauge != null)
            {
                viewModel.ProgressRatio
                    .Subscribe(ratio => progressGauge.Value = ratio * progressGauge.MaxValue)
                    .AddTo(disposables);
            }

            viewModel.ProgressText
                .Subscribe(text => progressText.text = text)
                .AddTo(disposables);

            viewModel.IsCompleted
                .Subscribe(completed => EnableInClassList(CompletedUssClassName, completed))
                .AddTo(disposables);

            return disposables;
        }
    }
}
