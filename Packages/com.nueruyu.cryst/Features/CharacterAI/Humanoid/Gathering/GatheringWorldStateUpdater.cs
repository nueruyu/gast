using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Gathering
{
    public class GatheringWorldStateUpdater : IWorldStateUpdater<GatheringState>
    {
        public void Update(ActorContext<GatheringState> context)
        {
            var memory = context.Memory;
            var actor = context.Actor;
            var state = context.WorldState;

            var currentGoal = memory.CurrentObjective;
            state.CurrentGoal = currentGoal;
            state.HasGoal = currentGoal != null;
            state.IsInCombat = memory.HasTarget;

            if (memory.InteractableTarget is Component interactableTargetComponent &&
                !interactableTargetComponent)
                memory.InteractableTarget = null;

            var interactableTarget = memory.InteractableTarget;

            state.HasInteractableTarget = interactableTarget != null;
            if (interactableTarget != null)
            {
                state.InteractableTargetId = interactableTarget.Id;
                state.InteractableTargetPosition = interactableTarget.Position;
                var distance = Vector3.Distance(actor.Body.Position,
                    interactableTarget.Position);
                state.IsInRangeToInteract = distance <= 1.5f;
            }
        }
    }
}
