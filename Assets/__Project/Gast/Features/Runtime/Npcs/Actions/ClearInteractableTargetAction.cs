using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace Gast.Features.Npcs.Actions
{
    public class ClearInteractableTargetAction : IAction<StrategicWorldState, AIContext<StrategicWorldState>>
    {
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
            ctx.Memory.InteractableTarget = null;
            return UniTask.CompletedTask;
        }
    }
}