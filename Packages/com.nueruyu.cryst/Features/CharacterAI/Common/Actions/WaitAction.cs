using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Core.Values;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Common.Actions
{
    public class WaitAction : IAction
    {
        readonly FloatRange duration;

        public WaitAction(FloatRange duration)
        {
            this.duration = duration;
        }

        public UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            return UniTask.Delay(
                TimeSpan.FromSeconds(duration.Sample()),
                cancellationToken: cancellationToken);
        }

        public override string ToString()
        {
            return $"{nameof(WaitAction)}({duration}s)";
        }
    }
}
