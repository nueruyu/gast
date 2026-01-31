using System;
using R3;
using UnityEngine.UIElements;

namespace Gast.UI.Hud
{
    public class AcquireItemObjectiveView : VisualElement
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

        public AcquireItemObjectiveView(VisualTreeAsset asset)
        {
            asset.CloneTree(this);
            icon = this.Q<VisualElement>(ItemIconName);
            description = this.Q<Label>(DescriptionName);
            progressBarFill = this.Q<VisualElement>(ProgressBarFillName);
            progressText = this.Q<Label>(ProgressTextName);
        }

        public IDisposable Bind(AcquireItemObjectiveViewModel viewModel)
        {
            var disposables = new CompositeDisposable();

            description.text = viewModel.Description;

            if (viewModel.ItemIcon != null)
            {
                icon.style.backgroundImage = new StyleBackground(viewModel.ItemIcon);
            }

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
