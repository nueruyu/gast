using System;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gast.Unity.UI.Interactions
{
    public class InteractionPromptView : VisualElement
    {
        const string KeyTextName = "key-text";
        const string ActionTextName = "action-text";
        const string ProgressCircleName = "progress-circle";
        const string ProgressFillName = "progress-fill";

        readonly Label keyText;
        readonly Label actionText;
        readonly VisualElement progressCircle;
        readonly VisualElement progressFill;

        Camera mainCamera;

        public InteractionPromptView(VisualTreeAsset asset)
        {
            asset.CloneTree(this);
            mainCamera = Camera.main;
            pickingMode = PickingMode.Ignore;

            keyText = this.Q<Label>(KeyTextName);
            actionText = this.Q<Label>(ActionTextName);
            progressCircle = this.Q<VisualElement>(ProgressCircleName);
            progressFill = this.Q<VisualElement>(ProgressFillName);

            style.display = DisplayStyle.None;
        }

        public IDisposable Bind(InteractionPromptViewModel viewModel)
        {
            var disposables = new CompositeDisposable();

            viewModel.IsVisible
                .Select(visible => visible ? DisplayStyle.Flex : DisplayStyle.None)
                .Subscribe(display => style.display = display)
                .AddTo(disposables);

            viewModel.KeyText
                .Subscribe(text => keyText.text = text)
                .AddTo(disposables);

            viewModel.ActionText
                .Subscribe(text => actionText.text = text)
                .AddTo(disposables);

            viewModel.IsHoldInteraction
                .Select(isHold => isHold ? DisplayStyle.Flex : DisplayStyle.None)
                .Subscribe(display => progressCircle.style.display = display)
                .AddTo(disposables);

            viewModel.HoldProgress
                .Select(progress => Length.Percent(progress * 100f))
                .Subscribe(width => progressFill.style.width = width)
                .AddTo(disposables);

            var positionUpdater = schedule
                .Execute(() => UpdatePosition(viewModel))
                .Every(0);

            disposables.Add(Disposable.Create(() =>
            {
                positionUpdater.Pause();
            }));

            return disposables;
        }

        void UpdatePosition(InteractionPromptViewModel viewModel)
        {
            if (panel == null || mainCamera == null) return;
            if (!viewModel.IsVisible.CurrentValue) return;

            var worldPos = viewModel.TargetWorldPosition.CurrentValue;
            var screenPos = mainCamera.WorldToScreenPoint(worldPos);

            if (screenPos.z < 0)
            {
                style.display = DisplayStyle.None;
                return;
            }

            style.display = DisplayStyle.Flex;

            var panelPos = RuntimePanelUtils.ScreenToPanel(
                panel,
                new Vector2(screenPos.x, Screen.height - screenPos.y));

            style.left = panelPos.x;
            style.top = panelPos.y;
        }
    }
}