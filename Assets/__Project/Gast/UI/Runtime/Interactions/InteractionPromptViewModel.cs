using System;
using Gast.Domain.Interactions;
using Gast.Domain.Players;
using Gast.Shared.Observables;
using R3;
using UnityEngine;

namespace Gast.UI.Interactions
{
    public class InteractionPromptViewModel : IDisposable
    {
        readonly CompositeDisposable disposables = new();
        readonly ReactiveProperty<float> holdProgress = new(0f);

        public ReadOnlyReactiveProperty<bool> IsVisible { get; }
        public ReadOnlyReactiveProperty<string> KeyText { get; }
        public ReadOnlyReactiveProperty<string> ActionText { get; }
        public ReadOnlyReactiveProperty<bool> IsHoldInteraction { get; }
        public ReadOnlyReactiveProperty<float> HoldProgress { get; }
        public ReadOnlyReactiveProperty<Vector3> TargetWorldPosition { get; }

        public InteractionPromptViewModel(IPlayerInteractionFocusService focusService, IInteractionSystem interactionSystem)
        {
            HoldProgress = holdProgress.ToReadOnlyReactiveProperty().AddTo(disposables);

            var currentInteractable = focusService.FocusedInteractable.ToObservable();

            IsVisible = currentInteractable
                .Select(x => x != null)
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            KeyText = currentInteractable
                .Select(x => x?.Config.Key ?? "")
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            ActionText = currentInteractable
                .Select(x => x?.Config.Prompt ?? "")
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            IsHoldInteraction = currentInteractable
                .Select(x => x?.Config.Type == InteractionType.Hold)
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            TargetWorldPosition = currentInteractable
                .Select(x => x?.Position ?? Vector3.zero)
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

            currentInteractable
                .Subscribe(_ => holdProgress.Value = 0f)
                .AddTo(disposables);

            interactionSystem.ProgressChanged.ToObservable()
                .Subscribe(evt =>
                {
                    var focused = focusService.FocusedInteractable.Value;
                    if (focused != null && evt.InteractableId == focused.Id)
                    {
                        holdProgress.Value = evt.Progress;
                    }
                })
                .AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
