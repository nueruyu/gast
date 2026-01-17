using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;

namespace Gast.Features.Npcs.Actions
{
    [Serializable]
    public class ClearTargetAction : IAction<StrategicWorldState, AIContext<StrategicWorldState>>
    {
        readonly SharedAIState sharedState;

        public ClearTargetAction(SharedAIState sharedState)
        {
            this.sharedState = sharedState;
        }

        public string Name => "ClearTarget";

        public bool CanExecute(StrategicWorldState worldState) => true;

        public void Simulate(StrategicWorldState worldState)
        {
        }

        public UniTask ExecuteAsync(AIContext<StrategicWorldState> ctx)
        {
            sharedState.CombatTarget = null;
            return UniTask.CompletedTask;
        }
    }
}
