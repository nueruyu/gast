using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Api.AI.Goals;
using Gast.Domain.Economy;
using Gast.Domain.Interactions;
using Gast.Domain.Pickups;
using Gast.Lib.AI;
using UnityEngine;

namespace Gast.Features.Npcs.Actions
{
    public class FindItemPickupAction : PrimitiveTask<StrategicWorldState>
    {
        readonly SharedAIState sharedState;
        readonly IPickupRepository pickupRepository;

        public FindItemPickupAction(
            SharedAIState sharedState,
            IPickupRepository pickupRepository) : base("FindItemPickupAction")
        {
            this.sharedState = sharedState;
            this.pickupRepository = pickupRepository;
        }

        protected override bool CheckCondition(StrategicWorldState state)
        {
            return state.CurrentGoal is AcquireItemGoal && !state.HasInteractableTarget;
        }

        protected override void ApplyEffect(ref StrategicWorldState state, ISimulationContext context)
        {
            state.HasInteractableTarget = true;
        }

        protected override async UniTask ExecuteAsync(Context<StrategicWorldState> ctx)
        {
            var goal = (AcquireItemGoal)ctx.CurrentState.CurrentGoal;

            var targetPickup = pickupRepository
                .GetAll()
                .Where(x =>
                {
                    return x.ItemId == goal.TargetItemId;
                })
                .OrderBy(x => Vector3.Distance(ctx.Character.Body.Position, x.Position))
                .FirstOrDefault();

            if (targetPickup != null)
            {
                sharedState.InteractableTarget = (targetPickup as Component).GetComponentInChildren<IInteractable>();
            }

            await UniTask.NextFrame(ctx.Token);
        }
    }
}