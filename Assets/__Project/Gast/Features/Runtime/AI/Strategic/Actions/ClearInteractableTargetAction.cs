using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace Gast.Features.AI.Strategic.Actions
{
    public class ClearInteractableTargetAction : IAction<StrategicState, AIContext<StrategicState>>
    {
        public bool CanExecute(StrategicState worldState)
        {
            return worldState.HasInteractableTarget;
        }

        public void Simulate(StrategicState worldState)
        {
            worldState.HasInteractableTarget = false;
            worldState.IsInRangeToInteract = false;
        }

        public UniTask ExecuteAsync(AIContext<StrategicState> ctx)
        {
            ctx.Memory.InteractableTarget = null;
            return UniTask.CompletedTask;
        }
    }
}