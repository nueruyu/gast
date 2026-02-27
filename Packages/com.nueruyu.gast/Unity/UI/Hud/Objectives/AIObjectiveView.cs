using System;
using R3;
using UnityEngine.UIElements;

namespace Gast.Unity.UI.Hud.Objectives
{
    public class AIObjectiveView : VisualElement
    {
        const string ItemIconName = "ItemIcon";
        const string DescriptionName = "Description";
        const string ProgressTextName = "ProgressText";
        const string ProgressBarFillName = "ProgressBarFill";
        const string CompletedUssClassName = "objective-item--completed";

        readonly VisualElement icon;
        readonly Label description;
        readonly Label progressText;
        readonly VisualElement progressBarFill;

        public AIObjectiveView(VisualTreeAsset asset)
        {
            asset.CloneTree(this);
            icon = this.Q<VisualElement>(ItemIconName);
            description = this.Q<Label>(DescriptionName);
            progressText = this.Q<Label>(ProgressTextName);
            progressBarFill = this.Q<VisualElement>(ProgressBarFillName);
        }

        public IDisposable Bind(IAIObjectiveViewModel viewModel)
        {
            var disposables = new CompositeDisposable();

            description.text = viewModel.Description;

            icon.style.backgroundImage = viewModel.ItemIcon != null
                ? new StyleBackground(viewModel.ItemIcon)
                : null;
            icon.style.display = viewModel.ItemIcon != null ? DisplayStyle.Flex : DisplayStyle.None;

            if (progressBarFill != null)
            {
                viewModel.ProgressRatio
                    .Subscribe(ratio => progressBarFill.style.width = Length.Percent(ratio * 100f))
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