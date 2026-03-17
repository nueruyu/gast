using System.Linq;
using System.Threading;
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
            return worldState.CurrentTargetItemId != null;
        }

        public void Simulate(GatheringState worldState)
        {
            worldState.MarkInteractableTargetFound();
        }

        public UniTask ExecuteAsync(ActorContext<GatheringState> context, CancellationToken cancellationToken)
        {
            if (context.WorldState.CurrentTargetItemId == null)
                return UniTask.CompletedTask;

            var targetItemId = context.WorldState.CurrentTargetItemId.Value;

            var targetPickup = context.PickupRepository
                .GetAll()
                .Where(x => x.ItemId == targetItemId)
                .OrderBy(x => Vector3.Distance(context.Actor.Body.Position, x.Position))
                .FirstOrDefault();

            if (targetPickup != null)
            {
                var targetPickupComponent = (Component)targetPickup;
                var memory = context.GetModule<HumanoidMemory>();
                memory.SetInteractableTarget(targetPickupComponent.GetComponentInChildren<IInteractable>());
            }

            return UniTask.CompletedTask;
        }
    }
}