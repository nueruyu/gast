using Cryst.Domain.AI.Objectives;
using Gast.Domain.Economy;
using Gast.Domain.Interactions;
using Gast.Lib.AI;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Gathering
{
    public class GatheringState : IWorldState<GatheringState>
    {
        public bool IsActive { get; private set; }
        public ItemId? CurrentTargetItemId { get; private set; }
        public InteractableId InteractableTargetId { get; private set; }
        public Vector3 InteractableTargetPosition { get; private set; }
        public bool HasInteractableTarget { get; private set; }
        public bool IsInRangeToInteract { get; private set; }

        public void WriteTo(ref GatheringState dest)
        {
            dest ??= new();
            dest.IsActive = IsActive;
            dest.CurrentTargetItemId = CurrentTargetItemId;
            dest.HasInteractableTarget = HasInteractableTarget;
            dest.InteractableTargetId = InteractableTargetId;
            dest.InteractableTargetPosition = InteractableTargetPosition;
            dest.IsInRangeToInteract = IsInRangeToInteract;
        }

        public void MarkInteractableTargetFound()
        {
            HasInteractableTarget = true;
        }

        public void MarkInRangeToInteract()
        {
            IsInRangeToInteract = true;
        }

        public void LostInteractableTarget()
        {
            HasInteractableTarget = false;
            IsInRangeToInteract = false;
        }

        public void Update(
            bool isActive,
            ItemId? currentTargetItemId,
            IInteractable interactableTarget,
            Vector3 actorPosition)
        {
            IsActive = isActive;
            CurrentTargetItemId = currentTargetItemId;

            HasInteractableTarget = interactableTarget != null;
            if (interactableTarget != null)
            {
                InteractableTargetId = interactableTarget.Id;
                InteractableTargetPosition = interactableTarget.Position;
                var distance = Vector3.Distance(actorPosition,
                    interactableTarget.Position);
                IsInRangeToInteract = distance <= 1.5f;
            }
        }
    }
}
