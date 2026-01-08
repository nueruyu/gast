using Gast.Core.Observables;
using Gast.Domain.Interactions;
using Gast.Features.Characters;
using UnityEngine;

namespace Gast.Features.Interactions
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

        InteractionSystem interactionSystem;
        bool activated = false;

        public InteractableId Id { get; private set; }
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

        public void Initialize(InteractionSystem interactionSystem)
        {
            this.interactionSystem = interactionSystem;

            if (activated)
            {
                interactionSystem.Register(this);
            }
        }

        void Awake()
        {
            Id = InteractableId.FromInstanceId(gameObject.GetInstanceID());
        }

        void OnEnable()
        {
            activated = true;
            interactionSystem?.Register(this);
        }

        void OnDisable()
        {
            interactionSystem?.Unregister(this);
            disabled.Publish();
            activated = false;
        }

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
    }
}