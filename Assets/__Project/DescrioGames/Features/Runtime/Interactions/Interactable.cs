using DescrioGames.Core.Observables;
using DescrioGames.Domain.Interactions;
using DescrioGames.Features.Characters;
using System;
using UnityEngine;

namespace DescrioGames.Features.Interactions
{
    public class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField]
        bool canInteract = true;

        [SerializeField]
        InteractionConfig config = new();

        readonly Signal<Character> interacted = new();
        readonly Signal<Character> interactionStarted = new();
        readonly Signal<Character> interactionCancelled = new();
        readonly Signal disabled = new();

        public InteractionConfig Config => config;
        public Vector3 Position => transform.position;

        public bool CanInteract
        {
            get => canInteract;
            set => canInteract = value;
        }

        public ISignal<Character> Interacted => interacted;

        public ISignal<Character> InteractionStarted => interactionStarted;

        public ISignal<Character> InteractionCancelled => interactionCancelled;
        public ISignal Disabled => disabled;

        IInteractionConfig IInteractable.Config => Config;

        public void OnInteract(Character interactor)
        {
            interacted.Publish(interactor);
        }

        public void OnInteractionStart(Character interactor)
        {
            interactionStarted.Publish(interactor);
        }

        public void OnInteractionCancelled(Character interactor)
        {
            interactionCancelled.Publish(interactor);
        }

        void OnDisable()
        {
            disabled.Publish();
        }
    }
}