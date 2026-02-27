using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Observables;
using Gast.Domain.Characters;
using Gast.Domain.Interactions;
using Gast.Features.Characters;
using UnityEngine;

namespace Gast.Features.Interactions
{
    public class InteractionSystem : IInteractionSystem
    {
        readonly Dictionary<InteractableId, Interactable> registeredInteractables = new();
        readonly ICharacterRepository characterRepository;
        readonly Signal<InteractionProgressEvent> progressChanged = new();

        public ISignal<InteractionProgressEvent> ProgressChanged => progressChanged;

        public InteractionSystem(ICharacterRepository characterRepository)
        {
            this.characterRepository = characterRepository;
        }

        public void Register(Interactable interactable)
        {
            registeredInteractables[interactable.Id] = interactable;
        }

        public void Unregister(Interactable interactable)
        {
            registeredInteractables.Remove(interactable.Id);
        }

        public async ValueTask<bool> RequestInteractionAsync(
            CharacterId interactorId,
            InteractableId interactableId,
            CancellationToken cancellationToken = default)
        {
            if (!registeredInteractables.TryGetValue(interactableId, out var interactable) || interactable == null)
                return false;

            var interactorCharacter = characterRepository.Get(interactorId);
            if (interactorCharacter == null)
                return false;

            if (!interactorCharacter.Is(out IInteractor interactor))
                return false;

            if (!interactor.Sensor.IsDetectable(interactableId) ||
                !interactable.CanInteract)
                return false;

            if (interactable.Config.Type == InteractionType.Instant)
            {
                interactable.OnInteract(interactorCharacter);
                return true;
            }

            if (interactable.Config.Type == InteractionType.Hold)
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                    interactable.destroyCancellationToken,
                    cancellationToken);

                try
                {
                    interactable.OnInteractionStart(interactorCharacter);

                    var elapsedTime = 0f;
                    var holdDuration = interactable.Config.HoldDuration;

                    while (elapsedTime < holdDuration)
                    {
                        linkedCts.Token.ThrowIfCancellationRequested();

                        elapsedTime += Time.deltaTime;
                        var progress = Mathf.Clamp01(elapsedTime / holdDuration);
                        NotifyProgress(interactorId, interactableId, progress);

                        await UniTask.Yield(PlayerLoopTiming.Update, linkedCts.Token);

                        var loopDistance = Vector3.Distance(interactor.InteractionPoint, interactable.Position);
                        if (!interactor.Sensor.IsDetectable(interactableId))
                        {
                            interactable.OnInteractionCancelled(interactorCharacter);
                            NotifyProgress(interactorId, interactableId, 0);
                            return false;
                        }
                    }

                    NotifyProgress(interactorId, interactableId, 1);

                    interactable.OnInteract(interactorCharacter);

                    NotifyProgress(interactorId, interactableId, 0);

                    return true;
                }
                catch (OperationCanceledException)
                {
                    if (interactable != null)
                        interactable.OnInteractionCancelled(interactorCharacter);
                    progressChanged.Publish(new InteractionProgressEvent(interactorId, interactableId, 0f));
                    return false;
                }
            }

            return false;
        }

        void NotifyProgress(CharacterId interactorId, InteractableId interactableId, float progress)
        {
            progressChanged.Publish(new InteractionProgressEvent(interactorId, interactableId, progress));
        }
    }
}