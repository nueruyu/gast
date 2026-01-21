using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;

namespace Gast.Features.AI.Strategic.Actions
{
    public class WaitAction : IAction<StrategicState, AIContext<StrategicState>, float>
    {
        public bool CanExecute(StrategicState worldState, float param)
        {
            return true;
        }

        public void Simulate(StrategicState worldState, float param)
        {
        }

        public UniTask ExecuteAsync(AIContext<StrategicState> ctx, float seconds)
        {
            return UniTask.Delay(
                TimeSpan.FromSeconds(seconds),
                cancellationToken: ctx.CancellationToken);
        }
    }
}