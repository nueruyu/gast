using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;

namespace Gast.Features.Npcs.Actions
{
    [Serializable]
    public class ClearTargetAction : PrimitiveTask<StrategicWorldState, AIContext<StrategicWorldState>>
    {
        readonly SharedAIState sharedState;

        public ClearTargetAction(SharedAIState sharedState) : base("ClearTarget")
        {
            this.sharedState = sharedState;
        }

        protected override bool CanExecute(StrategicWorldState worldState) => true;

        protected override void Simulate(StrategicWorldState worldState)
        {
        }

        protected override UniTask ExecuteAsync(AIContext<StrategicWorldState> ctx)
        {
            sharedState.CombatTarget = null;
            return UniTask.CompletedTask;
        }
    }
}
