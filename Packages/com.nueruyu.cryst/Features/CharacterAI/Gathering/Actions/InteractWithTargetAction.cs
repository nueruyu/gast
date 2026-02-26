using Cysharp.Threading.Tasks;
using Gast.Application.Interactions;
using Gast.Core.Commands;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Gathering.Actions
{
    public class InteractWithTargetAction : IAction<GatheringState, AIContext<GatheringState>>
    {
        readonly ICommandDispatcher commandDispatcher;

        public InteractWithTargetAction(ICommandDispatcher commandDispatcher)
        {
            this.commandDispatcher = commandDispatcher;
        }

        public bool CanExecute(GatheringState worldState)
        {
            return worldState.HasInteractableTarget && worldState.IsInRangeToInteract;
        }

        public void Simulate(GatheringState worldState)
        {
            worldState.HasInteractableTarget = false;
            worldState.IsInRangeToInteract = false;
        }

        public async UniTask ExecuteAsync(AIContext<GatheringState> ctx)
        {
            var command = new InteractCommand(ctx.Actor.Id, ctx.WorldState.InteractableTargetId);
            await commandDispatcher.DispatchAsync<InteractCommand, bool>(command, ctx.CancellationToken);
        }
    }
}