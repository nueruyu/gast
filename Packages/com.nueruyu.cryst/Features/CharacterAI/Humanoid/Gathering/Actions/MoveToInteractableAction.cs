using System.Threading;
using Cryst.Features.CharacterAI.Common;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Humanoid.Gathering.Actions
{
    public class MoveToInteractableAction : IAction<ActorContext<GatheringState>, GatheringState>
    {
        public bool IsAvailable(GatheringState worldState)
        {
            return worldState.HasInteractableTarget;
        }

        public void Simulate(GatheringState worldState)
        {
            worldState.IsInRangeToInteract = true;
        }

        public async UniTask ExecuteAsync(ActorContext<GatheringState> context, CancellationToken cancellationToken)
        {
            await context.Actor.MoveToAsync(
                static ctx => ctx.WorldState.InteractableTargetPosition,
                static ctx => ctx.WorldState.IsInRangeToInteract || !ctx.WorldState.HasInteractableTarget,
                context,
                cancellationToken);
        }
    }
}
