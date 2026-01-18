using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;

namespace Gast.Features.AI.Strategic.Actions
{
    [Serializable]
    public class ClearTargetAction : IAction<StrategicState, AIContext<StrategicState>>
    {
        public bool CanExecute(StrategicState worldState) => true;

        public void Simulate(StrategicState worldState)
        {
        }

        public UniTask ExecuteAsync(AIContext<StrategicState> ctx)
        {
            ctx.Memory.CombatTarget = null;
            return UniTask.CompletedTask;
        }
    }
}