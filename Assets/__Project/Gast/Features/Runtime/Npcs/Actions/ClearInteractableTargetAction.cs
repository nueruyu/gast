using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace Gast.Features.Npcs.Actions
{
    public class ClearInteractableTargetAction : IAction<StrategicWorldState, AIContext<StrategicWorldState>>
    {
        readonly SharedAIState sharedState;

        public ClearInteractableTargetAction(SharedAIState sharedState)
        {
            this.sharedState = sharedState;
        }

        public string Name => "ClearInteractableTargetAction";

        public bool CanExecute(StrategicWorldState worldState)
        {
            return worldState.HasInteractableTarget;
        }

        public void Simulate(StrategicWorldState worldState)
        {
            worldState.HasInteractableTarget = false;
            worldState.IsInRangeToInteract = false;
        }

        public UniTask ExecuteAsync(AIContext<StrategicWorldState> ctx)
        {
            sharedState.InteractableTarget = null;
            return UniTask.CompletedTask;
        }
    }
}
