using System.Linq;
using Cysharp.Threading.Tasks;
using Gast.Api.AI.Goals;
using Gast.Domain.Economy;
using Gast.Domain.Pickups;
using Gast.Lib.AI;
using UnityEngine;

namespace Gast.Features.Npcs.Actions
{
    public class FindItemPickupAction : PrimitiveTask<StrategicWorldState>
    {
        readonly SharedAIState sharedState;

        public FindItemPickupAction(SharedAIState sharedState) : base("FindItemPickupAction")
        {
            this.sharedState = sharedState;
        }

        protected override bool CheckCondition(StrategicWorldState state)
        {
            return state.CurrentGoal is AcquireItemGoal && !state.HasInteractableTarget;
        }

        protected override void ApplyEffect(ref StrategicWorldState state, ISimulationContext context)
        {
            state.HasInteractableTarget = true;
        }

        protected override UniTask ExecuteAsync(Context<StrategicWorldState> ctx)
        {
            var goal = (AcquireItemGoal)ctx.CurrentState.CurrentGoal;
            var sensor = ctx.Character.InteractionSensor;

            var targetPickup = sensor.DetectableInteractables
                .Where(x =>
                {
                    var pickup = (x as Component).GetComponentInParent<IPickup>();
                    if (pickup is null)
                        return false;
                    return pickup.ItemId == goal.TargetItemId;
                })
                .OrderBy(x => Vector3.Distance(ctx.Character.Body.Position, x.Position))
                .FirstOrDefault();

            if (targetPickup != null)
            {
                sharedState.InteractableTarget = targetPickup;
            }

            return UniTask.CompletedTask;
        }
    }
}