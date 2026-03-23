using Cryst.Domain.AI.Objectives;
using Gast.Domain.Economy;
using Gast.Domain.Interactions;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid
{
    public class GatheringMemory
    {
        AcquireItemObjective CurrentObjective { get; set; }
        public IInteractable InteractableTarget { get; private set; }

        public ItemId? TargetItemId => CurrentObjective?.TargetItemId;
        public bool HasGatheringTarget => InteractableTarget != null;

        public void SetObjective(AcquireItemObjective objective)
        {
            CurrentObjective = objective;
            InteractableTarget = null;
        }

        public void SetInteractableTarget(IInteractable interactable)
        {
            InteractableTarget = interactable;
        }

        public void Update()
        {
            if (InteractableTarget is Component interactableComponent && !interactableComponent)
                InteractableTarget = null;

            if (CurrentObjective?.IsCompleted.Value == true)
            {
                CurrentObjective = null;
                InteractableTarget = null;
            }
        }
    }
}
