using Cysharp.Threading.Tasks;
using Gast.Application.Interactions;
using Gast.Core.Commands;
using Gast.Lib.AI;
using System.Threading;
using ActorContext_ = Cryst.Features.CharacterAI.ActorContext<Cryst.Features.CharacterAI.Gathering.GatheringState>;

namespace Cryst.Features.CharacterAI.Gathering.Actions
{
    public class InteractWithTargetAction : IAction<ActorContext_, GatheringState>
    {
        public bool CanExecute(GatheringState worldState)
        {
            return worldState.HasInteractableTarget && worldState.IsInRangeToInteract;
        }

        public void Simulate(GatheringState worldState)
        {
            worldState.HasInteractableTarget = false;
            worldState.IsInRangeToInteract = false;
        }

        public async UniTask ExecuteAsync(ActorContext_ context, CancellationToken cancellationToken)
        {
            var command = new InteractCommand(context.Actor.Id, context.WorldState.InteractableTargetId);
            await context.CommandDispatcher.DispatchAsync<InteractCommand, bool>(command, cancellationToken);
        }
    }
}
