using System.Linq;
using System.Threading;
using Cryst.Domain.AI.Objectives;
using Cysharp.Threading.Tasks;
using Gast.Domain.Interactions;
using Gast.Lib.AI;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Gathering.Actions
{
    public class FindItemPickupAction : IAction<ActorContext<GatheringState>, GatheringState>
    {
        public bool IsAvailable(GatheringState worldState)
        {
            return worldState.CurrentGoal is AcquireItemObjective && !worldState.HasInteractableTarget;
        }

        public void Simulate(GatheringState worldState)
        {
            worldState.HasInteractableTarget = true;
        }

        public UniTask ExecuteAsync(ActorContext<GatheringState> context, CancellationToken cancellationToken)
        {
            var memory = context.GetModule<HumanoidMemory>();
            if (memory.CurrentObjective is not AcquireItemObjective goal) return UniTask.CompletedTask;

            var targetPickup = context.PickupRepository
                .GetAll()
                .Where(x => { return x.ItemId == goal.TargetItemId; })
                .OrderBy(x => Vector3.Distance(context.Actor.Body.Position, x.Position))
                .FirstOrDefault();

            if (targetPickup != null)
                memory.InteractableTarget = (targetPickup as Component).GetComponentInChildren<IInteractable>();

            return UniTask.CompletedTask;
        }
    }
}