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

        public ReadOnlyReactiveProperty<bool> IsVisible { get; }
        public ReadOnlyReactiveProperty<string> KeyText { get; }
        public ReadOnlyReactiveProperty<string> ActionText { get; }
        public ReadOnlyReactiveProperty<bool> IsHoldInteraction { get; }
        public ReadOnlyReactiveProperty<float> HoldProgress { get; }
        public ReadOnlyReactiveProperty<Vector3> TargetWorldPosition { get; }

        public InteractionPromptViewModel(IPlayerInteractionFocusService focusService)
        {
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

            // Note: This ViewModel no longer knows about hold progress.
            // This would require a more complex state propagation from InteractionSystem if needed.
            // For now, we assume the prompt disappears during the hold or shows no progress.
            HoldProgress = new ReactiveProperty<float>(0f).ToReadOnlyReactiveProperty();

            TargetWorldPosition = currentInteractable
                .Select(x => x?.Position ?? Vector3.zero)
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
