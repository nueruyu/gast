using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Observables;
using Gast.Core.Tasks;
using Gast.Domain.Characters;
using Gast.Domain.Interactions;
using Gast.Features.Characters;
using Gast.Shared.Observables;
using R3;
using UnityEngine;

namespace Gast.Features.Interactions
{
    /// <summary>
    /// Detects nearby interactable objects and manages interaction state.
    /// </summary>
    public class InteractionSystem : ILifecycleTask, IInteractionSystem
    {
        readonly Live<Interactable> currentInteractable = new(null);
        readonly Live<float> interactionProgress = new(0f);
        readonly Signal<IInteractable> interactionCompleted = new();
        readonly Dictionary<InteractableId, Interactable> registeredInteractables = new();

        readonly InteractionSystemSettings settings;
        readonly ICharacterActorRepository characterActorRepository;
        readonly Collider[] detectionColliderBuffer = new Collider[32];

        bool isProcessing;
        Character interactor;

        /// <summary>
        /// Observable property for the currently focused interactable.
        /// </summary>
        public ILive<IInteractable> CurrentInteractable { get; }

        /// <summary>
        /// Signal fired when an interaction completes.
        /// </summary>
        public ISignal<IInteractable> InteractionCompleted => interactionCompleted;

        /// <summary>
        /// Observable property for hold interaction progress (0.0 to 1.0).
        /// </summary>
        public ILive<float> InteractionProgress => interactionProgress;

        public InteractionSystem(InteractionSystemSettings settings, ICharacterActorRepository characterActorRepository)
        {
            this.settings = settings;
            this.characterActorRepository = characterActorRepository;

            CurrentInteractable = currentInteractable.Cast<Interactable, IInteractable>();

            currentInteractable.ToObservable()
                .Select(x => x != null ? x.Disabled.ToObservable() : Observable.Never<Unit>())
                .Switch()
                .Subscribe(_ =>
                {
                    currentInteractable.Value = null;
                });
        }

        public void Register(Interactable interactable)
        {
            registeredInteractables[interactable.Id] = interactable;
        }

        public void Unregister(Interactable interactable)
        {
            registeredInteractables.Remove(interactable.Id);
        }

        public void SetInteractor(CharacterId interactorId)
        {
            var interactor = characterActorRepository.Get(interactorId);
            SetInteractor(interactor);
        }

        public void UnsetInteractor()
        {
            SetInteractor(null);
        }

        void SetInteractor(Character newInteractor)
        {
            if (currentInteractable.Value != null)
            {
                CancelInteraction();
                currentInteractable.Value = null;
            }

            interactor = newInteractor;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            using (cancellationToken.Register(CancelInteraction))
            {
                await UniTask.WhenAll(
                    ProcessDetection(cancellationToken),
                    ProcessHoldInteraction(cancellationToken));
            }
        }

        async UniTask ProcessDetection(CancellationToken cancellationToken)
        {
            while (true)
            {
                await UniTask.Delay(
                    TimeSpan.FromSeconds(settings.DetectionInterval),
                    ignoreTimeScale: true,
                    delayTiming: PlayerLoopTiming.FixedUpdate,
                    cancellationToken: cancellationToken);

                if (interactor == null)
                    continue;

                var closest = FindClosestInteractable();

                if (closest != currentInteractable.Value)
                {
                    CancelInteraction();
                    currentInteractable.Value = closest;
                }
            }
        }

        async UniTask ProcessHoldInteraction(CancellationToken cancellationToken)
        {
            while (true)
            {
                await UniTask.NextFrame(cancellationToken);

                if (isProcessing && currentInteractable.Value != null)
                {
                    var progress = interactionProgress.Value + Time.deltaTime / currentInteractable.Value.Config.HoldDuration;
                    interactionProgress.Value = Mathf.Clamp01(progress);

                    if (progress >= 1f)
                    {
                        ExecuteInteraction();
                        isProcessing = false;
                        interactionProgress.Value = 0f;
                    }
                }
            }
        }

        /// <summary>
        /// Try to start an interaction with the current interactable.
        /// </summary>
        public bool TryInteract()
        {
            if (currentInteractable.Value == null ||
                !currentInteractable.Value.CanInteract)
                return false;

            switch (currentInteractable.Value.Config.Type)
            {
                case InteractionType.Instant:
                    ExecuteInteraction();
                    break;

                case InteractionType.Hold:
                    if (isProcessing)
                        return false;
                    StartHoldInteraction();
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return true;
        }

        /// <summary>
        /// Cancel the current interaction (for hold interactions).
        /// </summary>
        public void CancelInteraction()
        {
            if (isProcessing && currentInteractable.Value)
            {
                currentInteractable.Value.OnInteractionCancelled(interactor);
            }

            isProcessing = false;
            interactionProgress.Value = 0f;
        }

        public async ValueTask<bool> RequestInteractionAsync(CharacterId interactorId, InteractableId interactableId)
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
                interactionCompleted.Publish(interactable);
                return true;
            }
            if (interactable.Config.Type == InteractionType.Hold)
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(interactable.destroyCancellationToken);
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
                    interactionCompleted.Publish(interactable);
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

        void StartHoldInteraction()
        {
            if (isProcessing)
                throw new InvalidOperationException();
            if (currentInteractable.Value == null)
                throw new InvalidOperationException();

            isProcessing = true;
            interactionProgress.Value = 0f;

            currentInteractable.Value.OnInteractionStart(interactor);
        }

        void ExecuteInteraction()
        {
            if (currentInteractable.Value == null)
                throw new InvalidOperationException();

            currentInteractable.Value.OnInteract(interactor);

            interactionProgress.Value = 0f;
            interactionCompleted.Publish(currentInteractable.Value);
        }

        Interactable FindClosestInteractable()
        {
            var origin = interactor.transform;
            var hitCount = Physics.OverlapSphereNonAlloc(
                origin.position,
                settings.DetectionRadius,
                detectionColliderBuffer,
                settings.InteractableLayer);

            Interactable closest = null;
            var closestDistanceSqr = float.MaxValue;
            for (int i = 0; i < hitCount; i++)
            {
                var col = detectionColliderBuffer[i];

                var interactable = col.GetComponentInParent<Interactable>();
                if (interactable == null || !interactable.CanInteract)
                    continue;

                var distanceSqr = (origin.position - col.ClosestPoint(origin.position)).sqrMagnitude;
                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closest = interactable;
                }
            }

            return closest;
        }
    }
}