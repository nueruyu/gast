using System.Threading;
using Cryst.Domain.AI.Objectives;
using Cysharp.Threading.Tasks;
using Gast.Domain.Interactions;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Objective.Actions
{
    public class SelectGatheringObjectiveAction : SelectObjectiveAction<AcquireItemObjective>
    {
        public SelectGatheringObjectiveAction(AcquireItemObjective objective) : base(objective) { }

        public override UniTask ExecuteAsync(ActorContext<ObjectiveState> context, CancellationToken cancellationToken)
        {
            var pickupInfo = context.ObjectiveQueries.FindBestTargetFor(objective, context.WorldState);
            if (!pickupInfo.HasValue)
                return UniTask.CompletedTask;

            var pickup = context.PickupRepository.Find(pickupInfo.Value.Id);
            var interactable = ((Component)pickup)?.GetComponentInChildren<IInteractable>();
            if (interactable != null)
            {
                var memory = context.GetModule<GatheringMemory>();
                memory.SetObjective(objective);
                memory.SetInteractableTarget(interactable);
            }

            return UniTask.CompletedTask;
        }
    }
}
