using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Application.Interactions;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Humanoid.Gathering.Actions
{
    public class InteractWithTargetAction : IAction<ActorContext<GatheringState>, GatheringState>
    {
        public bool IsAvailable(GatheringState worldState)
        {
            return worldState.HasInteractableTarget && worldState.IsInRangeToInteract;
        }

        public void Simulate(GatheringState worldState)
        {
            worldState.LostInteractableTarget();
        }

        public async UniTask ExecuteAsync(ActorContext<GatheringState> context, CancellationToken cancellationToken)
        {
            var command = new InteractCommand(context.Actor.Id, context.WorldState.InteractableTargetId);
            var success =
                await context.CommandDispatcher.DispatchAsync<InteractCommand, bool>(command, cancellationToken);

            if (success)
            {
                var memory = context.GetModule<GatheringMemory>();
                memory.SetInteractableTarget(null);
            }
        }
    }
}
