using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cryst.Domain.AI.Objectives;
using Gast.Domain.Interactions;
using Gast.Domain.Pickups;
using Gast.Lib.AI;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Gathering.Actions
{
    public class FindItemPickupAction : IAction<ActorContext<GatheringState>, GatheringState>
    {
        public bool CanExecute(GatheringState worldState)
        {
            return worldState.CurrentGoal is AcquireItemObjective && !worldState.HasInteractableTarget;
        }

        public void Simulate(GatheringState worldState)
        {
            worldState.HasInteractableTarget = true;
        }

        public UniTask ExecuteAsync(ActorContext<GatheringState> context, CancellationToken cancellationToken)
        {
            if (context.Memory.CurrentObjective is not AcquireItemObjective goal)
            {
                return UniTask.CompletedTask;
            }

            var targetPickup = context.PickupRepository
                .GetAll()
                .Where(x =>
                {
                    return x.ItemId == goal.TargetItemId;
                })
                .OrderBy(x => Vector3.Distance(context.Actor.Body.Position, x.Position))
                .FirstOrDefault();

            if (targetPickup != null)
            {
                context.Memory.InteractableTarget = (targetPickup as Component).GetComponentInChildren<IInteractable>();
            }

            return UniTask.CompletedTask;
        }
    }
}
