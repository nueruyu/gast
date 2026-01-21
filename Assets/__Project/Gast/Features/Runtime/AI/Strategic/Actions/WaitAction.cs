using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;

namespace Gast.Features.AI.Strategic.Actions
{
    public class WaitAction : IAction<StrategicState, AIContext<StrategicState>>
    {
        readonly float seconds;

        public WaitAction(float seconds)
        {
            this.seconds = seconds;
        }

        public bool CanExecute(StrategicState worldState)
        {
            return true;
        }

        public void Simulate(StrategicState worldState)
        {
        }

        public UniTask ExecuteAsync(AIContext<StrategicState> ctx)
        {
            return UniTask.Delay(
                TimeSpan.FromSeconds(seconds),
                cancellationToken: ctx.CancellationToken);
        }
    }
}