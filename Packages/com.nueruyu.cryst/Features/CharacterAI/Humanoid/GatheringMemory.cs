using Cryst.Domain.AI.Objectives;
using Gast.Domain.Economy;
using Gast.Domain.Interactions;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid
{
    public class GatheringMemory
    {
        IInteractable interactableTarget;
        AcquireItemObjective CurrentObjective { get; set; }

        public IInteractable InteractableTarget
        {
            get
            {
                if (interactableTarget is Component interactableComponent && !interactableComponent)
                    interactableTarget = null;
                return interactableTarget;
            }
        }

        public ItemId? TargetItemId => CurrentObjective?.TargetItemId;
        public bool HasGatheringTarget => InteractableTarget != null;

        public void SetObjective(AcquireItemObjective objective, IInteractable interactable)
        {
            CurrentObjective = objective;
            interactableTarget = interactable;
        }

        public void Update()
        {
            if (CurrentObjective?.IsCompleted.Value == true)
            {
                CurrentObjective = null;
                interactableTarget = null;
            }
        }
    }
}