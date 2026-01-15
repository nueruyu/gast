using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace Gast.Features.Npcs.Actions
{
    public class ClearInteractableTargetAction : PrimitiveTask<StrategicWorldState, AIContext<StrategicWorldState>>
    {
        readonly SharedAIState sharedState;

        public ClearInteractableTargetAction(SharedAIState sharedState) : base("ClearInteractableTargetAction")
        {
            this.sharedState = sharedState;
        }

        protected override bool CanExecute(StrategicWorldState worldState)
        {
            return worldState.HasInteractableTarget;
        }

        protected override void Simulate(StrategicWorldState worldState)
        {
            worldState.HasInteractableTarget = false;
            worldState.IsInRangeToInteract = false;
        }

        protected override UniTask ExecuteAsync(AIContext<StrategicWorldState> ctx)
        {
            sharedState.InteractableTarget = null;
            return UniTask.CompletedTask;
        }
    }
}
