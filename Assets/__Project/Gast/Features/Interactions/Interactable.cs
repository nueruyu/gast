using Gast.Core.Observables;
using Gast.Domain.Characters;
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

        readonly Signal<ICharacter> interacted = new();
        readonly Signal<ICharacter> interactionStarted = new();
        readonly Signal<ICharacter> interactionCancelled = new();
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

        public ISignal<ICharacter> Interacted => interacted;
        public ISignal<ICharacter> InteractionStarted => interactionStarted;
        public ISignal<ICharacter> InteractionCancelled => interactionCancelled;
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

        public void OnInteract(ICharacter interactor)
        {
            interacted.Publish(interactor);
        }

        public void OnInteractionStart(ICharacter interactor)
        {
            interactionStarted.Publish(interactor);
        }

        public void OnInteractionCancelled(ICharacter interactor)
        {
            interactionCancelled.Publish(interactor);
        }
    }
}