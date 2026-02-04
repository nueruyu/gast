using System.Linq;
using Cysharp.Threading.Tasks;
using Gast.Domain.AI.Objectives;
using Gast.Domain.Interactions;
using Gast.Domain.Pickups;
using Gast.Lib.AI;
using UnityEngine;

namespace Gast.Features.AI.Gathering.Actions
{
    public class FindItemPickupAction : IAction<GatheringState, AIContext<GatheringState>>
    {
        readonly IPickupRepository pickupRepository;

        public FindItemPickupAction(IPickupRepository pickupRepository)
        {
            this.pickupRepository = pickupRepository;
        }

        public bool CanExecute(GatheringState worldState)
        {
            return worldState.CurrentGoal is AcquireItemObjective && !worldState.HasInteractableTarget;
        }

        public void Simulate(GatheringState worldState)
        {
            worldState.HasInteractableTarget = true;
        }

        public UniTask ExecuteAsync(AIContext<GatheringState> ctx)
        {
            if (ctx.Memory.CurrentObjective is not AcquireItemObjective goal)
            {
                return UniTask.CompletedTask;
            }

            var targetPickup = pickupRepository
                .GetAll()
                .Where(x =>
                {
                    return x.ItemId == goal.TargetItemId;
                })
                .OrderBy(x => Vector3.Distance(ctx.Actor.Body.Position, x.Position))
                .FirstOrDefault();

            if (targetPickup != null)
            {
                ctx.Memory.InteractableTarget = (targetPickup as Component).GetComponentInChildren<IInteractable>();
            }

            return UniTask.CompletedTask;
        }
    }
}