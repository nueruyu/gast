using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace GastGame.Features.AI.Gathering.Actions
{
    public class ClearInteractableTargetAction : IAction<GatheringState, AIContext<GatheringState>>
    {
        public bool CanExecute(GatheringState worldState)
        {
            return worldState.HasInteractableTarget;
        }

        public void Simulate(GatheringState worldState)
        {
            worldState.HasInteractableTarget = false;
            worldState.IsInRangeToInteract = false;
        }

        public UniTask ExecuteAsync(AIContext<GatheringState> ctx)
        {
            ctx.Memory.InteractableTarget = null;
            return UniTask.CompletedTask;
        }
    }
}