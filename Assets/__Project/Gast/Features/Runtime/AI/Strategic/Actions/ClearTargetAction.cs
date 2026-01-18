using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;

namespace Gast.Features.AI.Strategic.Actions
{
    [Serializable]
    public class ClearTargetAction : IAction<StrategicWorldState, AIContext<StrategicWorldState>>
    {
        public string Name => "ClearTarget";

        public bool CanExecute(StrategicWorldState worldState) => true;

        public void Simulate(StrategicWorldState worldState)
        {
        }

        public UniTask ExecuteAsync(AIContext<StrategicWorldState> ctx)
        {
            ctx.Memory.CombatTarget = null;
            return UniTask.CompletedTask;
        }
    }
}