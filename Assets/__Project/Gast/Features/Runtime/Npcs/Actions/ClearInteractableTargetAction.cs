using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace Gast.Features.Npcs.Actions
{
    public class ClearInteractableTargetAction : PrimitiveTask<StrategicWorldState>
    {
        readonly SharedAIState sharedState;

        public ClearInteractableTargetAction(SharedAIState sharedState) : base("ClearInteractableTargetAction")
        {
            this.sharedState = sharedState;
        }

        protected override bool CheckCondition(StrategicWorldState state)
        {
            return state.HasInteractableTarget;
        }

        protected override void ApplyEffect(ref StrategicWorldState state, ISimulationContext context)
        {
            state.HasInteractableTarget = false;
            state.IsInRangeToInteract = false;
        }

        protected override UniTask ExecuteAsync(Context<StrategicWorldState> ctx)
        {
            sharedState.InteractableTarget = null;
            return UniTask.CompletedTask;
        }
    }
}
