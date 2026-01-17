using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;

namespace Gast.Features.Npcs.Actions
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
            ctx.SharedState.CombatTarget = null;
            return UniTask.CompletedTask;
        }
    }
}
