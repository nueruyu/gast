using System.Linq;
using Cysharp.Threading.Tasks;
using Gast.Domain.AI.Goals;
using Gast.Domain.Interactions;
using Gast.Domain.Pickups;
using Gast.Lib.AI;
using UnityEngine;

namespace Gast.Features.AI.Strategic.Actions
{
    public class FindItemPickupAction : IAction<StrategicState, AIContext<StrategicState>>
    {
        readonly IPickupRepository pickupRepository;

        public FindItemPickupAction(IPickupRepository pickupRepository)
        {
            this.pickupRepository = pickupRepository;
        }

        public bool CanExecute(StrategicState worldState)
        {
            return worldState.CurrentGoal is AcquireItemGoal && !worldState.HasInteractableTarget;
        }

        public void Simulate(StrategicState worldState)
        {
            worldState.HasInteractableTarget = true;
        }

        public async UniTask ExecuteAsync(AIContext<StrategicState> ctx)
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
                ctx.Memory.InteractableTarget = (targetPickup as Component).GetComponentInChildren<IInteractable>();
            }

            await UniTask.NextFrame(ctx.CancellationToken);
        }
    }
}