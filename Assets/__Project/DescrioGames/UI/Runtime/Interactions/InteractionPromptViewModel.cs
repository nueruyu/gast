using System;
using DescrioGames.Domain.Interactions;
using DescrioGames.Shared.Observables;
using R3;
using UnityEngine;

namespace DescrioGames.UI.Interactions
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

        public InteractionPromptViewModel(IInteractionSystem interactionSystem)
        {
            var currentInteractable = interactionSystem.CurrentInteractable.ToObservable();

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

            HoldProgress = interactionSystem
                .InteractionProgress
                .ToObservable()
                .ToReadOnlyReactiveProperty()
                .AddTo(disposables);

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