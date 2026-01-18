using Cysharp.Threading.Tasks;
using Gast.Api.Interactions;
using Gast.Core.Commands;
using Gast.Lib.AI;

namespace Gast.Features.AI.Strategic.Actions
{
    public class InteractWithTargetAction : IAction<StrategicState, AIContext<StrategicState>>
    {
        readonly ICommandDispatcher commandDispatcher;

        public InteractWithTargetAction(ICommandDispatcher commandDispatcher)
        {
            this.commandDispatcher = commandDispatcher;
        }

        public bool CanExecute(StrategicState worldState)
        {
            return worldState.HasInteractableTarget && worldState.IsInRangeToInteract;
        }

        public void Simulate(StrategicState worldState)
        {
            worldState.HasInteractableTarget = false;
            worldState.IsInRangeToInteract = false;
        }

        public async UniTask ExecuteAsync(AIContext<StrategicState> ctx)
        {
            var command = new InteractCommand(ctx.Actor.Id, ctx.WorldState.InteractableTargetId);
            await commandDispatcher.DispatchAsync<InteractCommand, bool>(command);
        }
    }
}