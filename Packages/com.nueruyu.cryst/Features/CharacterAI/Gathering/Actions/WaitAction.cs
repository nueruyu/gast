using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;

namespace Cryst.Features.CharacterAI.Gathering.Actions
{
    public class WaitAction : IAction<GatheringState, AIContext<GatheringState>, float>
    {
        public bool CanExecute(GatheringState worldState, float param)
        {
            return true;
        }

        public void Simulate(GatheringState worldState, float param)
        {
        }

        public UniTask ExecuteAsync(AIContext<GatheringState> ctx, float seconds)
        {
            return UniTask.Delay(
                TimeSpan.FromSeconds(seconds),
                cancellationToken: ctx.CancellationToken);
        }
    }
}