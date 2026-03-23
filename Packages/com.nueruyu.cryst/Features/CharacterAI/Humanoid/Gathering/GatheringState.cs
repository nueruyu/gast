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

        public void Update(ActorContext<GatheringState> context)
        {
            var memory = context.GetModule<HumanoidMemory>();
            var actor = context.Actor;

            IsActive = memory.CurrentMode == AIMode.Gathering;

            var acquireItemObjective = memory.CurrentObjective as AcquireItemObjective;
            CurrentTargetItemId = acquireItemObjective?.TargetItemId;

            var interactableTarget = memory.InteractableTarget;

            HasInteractableTarget = interactableTarget != null;
            if (interactableTarget != null)
            {
                InteractableTargetId = interactableTarget.Id;
                InteractableTargetPosition = interactableTarget.Position;
                var distance = Vector3.Distance(actor.Body.Position,
                    interactableTarget.Position);
                IsInRangeToInteract = distance <= 1.5f;
            }
        }
    }
}