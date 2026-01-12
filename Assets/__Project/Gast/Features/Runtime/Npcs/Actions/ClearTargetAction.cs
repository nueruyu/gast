using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;

namespace Gast.Features.Npcs.Actions
{
    [Serializable]
    public class ClearTargetAction : PrimitiveTask<StrategicWorldState>
    {
        readonly SharedAIState sharedState;

        public ClearTargetAction(SharedAIState sharedState) : base("ClearTarget")
        {
            this.sharedState = sharedState;
        }

        protected override bool CanExecute(StrategicWorldState state) => true;

        protected override void Simulate(ref StrategicWorldState state)
        {
        }

        protected override UniTask ExecuteAsync(Context<StrategicWorldState> ctx)
        {
            sharedState.CombatTarget = null;
            return UniTask.CompletedTask;
        }
    }
}