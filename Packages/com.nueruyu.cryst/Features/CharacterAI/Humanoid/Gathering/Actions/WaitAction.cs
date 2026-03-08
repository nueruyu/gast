using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Humanoid.Gathering.Actions
{
    public class WaitAction : IAction<ActorContext<GatheringState>, GatheringState>
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

        public UniTask ExecuteAsync(ActorContext<GatheringState> context, CancellationToken cancellationToken)
        {
            return UniTask.Delay(
                TimeSpan.FromSeconds(seconds),
                cancellationToken: cancellationToken);
        }

        public override string ToString()
        {
            return $"{nameof(WaitAction)}({seconds:F1}s)";
        }
    }
}
