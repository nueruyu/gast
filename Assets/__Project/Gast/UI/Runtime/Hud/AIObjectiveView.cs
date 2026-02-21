using System;
using R3;
using UnityEngine.UIElements;

namespace Gast.UI.Hud
{
    public class AIObjectiveView : VisualElement
    {
        const string ItemIconName = "ItemIcon";
        const string DescriptionName = "Description";
        const string ProgressBarFillName = "ProgressBarFill";
        const string ProgressTextName = "ProgressText";
        const string CompletedUssClassName = "objective-item--completed";

        readonly VisualElement icon;
        readonly Label description;
        readonly VisualElement progressBarFill;
        readonly Label progressText;

        public AIObjectiveView(VisualTreeAsset asset)
        {
            asset.CloneTree(this);
            icon = this.Q<VisualElement>(ItemIconName);
            description = this.Q<Label>(DescriptionName);
            progressBarFill = this.Q<VisualElement>(ProgressBarFillName);
            progressText = this.Q<Label>(ProgressTextName);
        }

        public IDisposable Bind(IAIObjectiveViewModel viewModel)
        {
            var disposables = new CompositeDisposable();

            description.text = viewModel.Description;

            icon.style.backgroundImage = viewModel.ItemIcon != null
                ? new StyleBackground(viewModel.ItemIcon)
                : null;
            icon.style.display = viewModel.ItemIcon != null ? DisplayStyle.Flex : DisplayStyle.None;

            viewModel.ProgressRatio
                .Subscribe(ratio => progressBarFill.style.width = Length.Percent(ratio * 100f))
                .AddTo(disposables);

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
