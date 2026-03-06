using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;
using System.Threading;
using ActorContext_ = Cryst.Features.CharacterAI.ActorContext<Cryst.Features.CharacterAI.Strategic.StrategicState>;

namespace Cryst.Features.CharacterAI.Strategic.Actions
{
    public class WaitAction : IAction<ActorContext_, StrategicState>
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

        public UniTask ExecuteAsync(ActorContext_ context, CancellationToken cancellationToken)
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
