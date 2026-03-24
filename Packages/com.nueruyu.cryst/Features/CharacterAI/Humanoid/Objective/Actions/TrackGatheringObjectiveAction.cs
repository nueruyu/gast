using System.Threading;
using Cryst.Domain.AI.Objectives;
using Cysharp.Threading.Tasks;
using Gast.Domain.Interactions;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Objective.Actions
{
    public class TrackGatheringObjectiveAction : TrackObjectiveAction<AcquireItemObjective>
    {
        public TrackGatheringObjectiveAction(AcquireItemObjective objective) : base(objective)
        {
        }

        public override async UniTask ExecuteAsync(ActorContext<ObjectiveState> context,
            CancellationToken cancellationToken)
        {
            var memory = context.GetModule<GatheringMemory>();

            while (!cancellationToken.IsCancellationRequested)
            {
                if (!memory.HasGatheringTarget)
                {
                    var pickupInfo = context.ObjectiveQueries.FindBestTargetFor(objective, context.WorldState);
                    if (pickupInfo.HasValue)
                    {
                        var pickup = context.PickupRepository.Find(pickupInfo.Value.Id);
                        var interactable = ((Component)pickup)?.GetComponentInChildren<IInteractable>();
                        if (interactable != null) memory.SetObjective(objective, interactable);
                    }
                }

                await UniTask.Yield(cancellationToken);
            }
        }
    }
}