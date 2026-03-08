using UnityEngine;

namespace Cryst.Features.CharacterAI.Gathering
{
    public class GatheringWorldStateUpdater : IWorldStateUpdater<GatheringState>
    {
        public void Update(ActorContext<GatheringState> context)
        {
            var memory = context.Memory;
            var actor = context.Actor;
            var gatheringState = context.WorldState;

            var currentGoal = memory.CurrentObjective;
            gatheringState.CurrentGoal = currentGoal;
            gatheringState.HasGoal = currentGoal != null;
            gatheringState.IsInCombat = memory.HasTarget;

            if (memory.InteractableTarget is Component interactableTargetComponent &&
                !interactableTargetComponent)
                memory.InteractableTarget = null;

            var interactableTarget = memory.InteractableTarget;

            gatheringState.HasInteractableTarget = interactableTarget != null;
            if (interactableTarget != null)
            {
                gatheringState.InteractableTargetId = interactableTarget.Id;
                gatheringState.InteractableTargetPosition = interactableTarget.Position;
                var distance = Vector3.Distance(actor.Body.Position,
                    interactableTarget.Position);
                gatheringState.IsInRangeToInteract = distance <= 1.5f;
            }
        }
    }
}
