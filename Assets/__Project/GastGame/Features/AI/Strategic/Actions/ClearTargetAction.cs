using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace GastGame.Features.AI.Strategic.Actions
{
    public class ClearTargetAction : IAction<StrategicState, AIContext<StrategicState>>
    {
        public bool CanExecute(StrategicState worldState)
        {
            return true;
        }

        public void Simulate(StrategicState worldState)
        {
            // This is a cognitive action that clears memory,
            // it does not change the predictable future state of the world for planning.
        }

        public UniTask ExecuteAsync(AIContext<StrategicState> ctx)
        {
            ctx.Memory.CombatTarget = null;
            ctx.Memory.CurrentObjective = null;
            ctx.Memory.InteractableTarget = null;
            return UniTask.CompletedTask;
        }
    }
}