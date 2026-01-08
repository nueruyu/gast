using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Domain.Characters;
using Gast.Domain.Interactions;
using Gast.Features.Characters;
using UnityEngine;

namespace Gast.Features.Interactions
{
    public class InteractionSystem : IInteractionSystem
    {
        readonly Dictionary<InteractableId, Interactable> registeredInteractables = new();
        readonly InteractionSystemSettings settings;
        readonly ICharacterActorRepository characterActorRepository;

        public InteractionSystem(InteractionSystemSettings settings, ICharacterActorRepository characterActorRepository)
        {
            this.settings = settings;
            this.characterActorRepository = characterActorRepository;
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

            var interactorCharacter = characterActorRepository.Get(interactorId);
            if (interactorCharacter == null)
                return false;

            var distance = Vector3.Distance(interactorCharacter.Body.Position, interactable.Position);
            if (distance > settings.DetectionRadius || !interactable.CanInteract)
                return false;

            if (interactable.Config.Type == InteractionType.Instant)
            {
                interactable.OnInteract(interactorCharacter);
                return true;
            }
            if (interactable.Config.Type == InteractionType.Hold)
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken,
                    interactable.destroyCancellationToken);
                try
                {
                    interactable.OnInteractionStart(interactorCharacter);
                    var holdDuration = TimeSpan.FromSeconds(interactable.Config.HoldDuration);
                    await UniTask.Delay(holdDuration, cancellationToken: linkedCts.Token);

                    var finalDistance = Vector3.Distance(interactorCharacter.Body.Position, interactable.Position);
                    if (finalDistance > settings.DetectionRadius || !interactable.CanInteract)
                    {
                        interactable.OnInteractionCancelled(interactorCharacter);
                        return false;
                    }

                    interactable.OnInteract(interactorCharacter);
                    return true;
                }
                catch (OperationCanceledException)
                {
                    if (interactable != null)
                        interactable.OnInteractionCancelled(interactorCharacter);
                    return false;
                }
            }

            return false;
        }
    }
}