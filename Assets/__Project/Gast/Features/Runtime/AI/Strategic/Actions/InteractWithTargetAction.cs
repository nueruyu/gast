using Cysharp.Threading.Tasks;
using Gast.Api.Interactions;
using Gast.Core.Commands;
using Gast.Lib.AI;

namespace Gast.Features.AI.Strategic.Actions
{
    public class InteractWithTargetAction : IAction<StrategicWorldState, AIContext<StrategicWorldState>>
    {
        readonly ICommandDispatcher commandDispatcher;

        public InteractWithTargetAction(ICommandDispatcher commandDispatcher)
        {
            this.commandDispatcher = commandDispatcher;
        }

        public string Name => "InteractWithTargetAction";

        public bool CanExecute(StrategicWorldState worldState)
        {
            return worldState.HasInteractableTarget && worldState.IsInRangeToInteract;
        }

        public void Simulate(StrategicWorldState worldState)
        {
            worldState.HasInteractableTarget = false;
            worldState.IsInRangeToInteract = false;
        }

        public async UniTask ExecuteAsync(AIContext<StrategicWorldState> ctx)
        {
            var command = new InteractCommand(ctx.Actor.Id, ctx.WorldState.InteractableTargetId);
            await commandDispatcher.DispatchAsync<InteractCommand, bool>(command);
        }
    }
}