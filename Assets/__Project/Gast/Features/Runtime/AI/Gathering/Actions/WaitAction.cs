using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;

namespace Gast.Features.AI.Gathering.Actions
{
    public class WaitAction : IAction<GatheringState, AIContext<GatheringState>>
    {
        readonly float seconds;

        public WaitAction(float seconds)
        {
            this.seconds = seconds;
        }

        public bool CanExecute(GatheringState worldState)
        {
            return true;
        }

        public void Simulate(GatheringState worldState)
        {
        }

        public UniTask ExecuteAsync(AIContext<GatheringState> ctx)
        {
            return UniTask.Delay(
                TimeSpan.FromSeconds(seconds),
                cancellationToken: ctx.CancellationToken);
        }
    }
}
