using System.Linq;
using Cysharp.Threading.Tasks;
using Gast.Api.AI.Goals;
using Gast.Domain.Interactions;
using Gast.Domain.Pickups;
using Gast.Lib.AI;
using UnityEngine;

namespace Gast.Features.Npcs.Actions
{
    public class FindItemPickupAction : IAction<StrategicWorldState, AIContext<StrategicWorldState>>
    {
        readonly SharedAIState sharedState;
        readonly IPickupRepository pickupRepository;

        public FindItemPickupAction(
            SharedAIState sharedState,
            IPickupRepository pickupRepository)
        {
            this.sharedState = sharedState;
            this.pickupRepository = pickupRepository;
        }

        public string Name => "FindItemPickupAction";

        public bool CanExecute(StrategicWorldState worldState)
        {
            return worldState.CurrentGoal is AcquireItemGoal && !worldState.HasInteractableTarget;
        }

        public void Simulate(StrategicWorldState worldState)
        {
            worldState.HasInteractableTarget = true;
        }

        public async UniTask ExecuteAsync(AIContext<StrategicWorldState> ctx)
        {
            var goal = (AcquireItemGoal)ctx.WorldState.CurrentGoal;

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
                sharedState.InteractableTarget = (targetPickup as Component).GetComponentInChildren<IInteractable>();
            }

            await UniTask.NextFrame(ctx.CancellationToken);
        }
    }
}
