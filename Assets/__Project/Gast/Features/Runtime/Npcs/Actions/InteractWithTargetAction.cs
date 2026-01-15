using Cysharp.Threading.Tasks;
using Gast.Api.Interactions;
using Gast.Core.Commands;
using Gast.Lib.AI;

namespace Gast.Features.Npcs.Actions
{
    public class InteractWithTargetAction : PrimitiveTask<StrategicWorldState, AIContext<StrategicWorldState>>
    {
        readonly ICommandDispatcher commandDispatcher;

        public InteractWithTargetAction(ICommandDispatcher commandDispatcher) : base("InteractWithTargetAction")
        {
            this.commandDispatcher = commandDispatcher;
        }

        protected override bool CanExecute(StrategicWorldState worldState)
        {
            return worldState.HasInteractableTarget && worldState.IsInRangeToInteract;
        }

        protected override void Simulate(StrategicWorldState worldState)
        {
            worldState.HasInteractableTarget = false;
            worldState.IsInRangeToInteract = false;
        }

        protected override async UniTask ExecuteAsync(AIContext<StrategicWorldState> ctx)
        {
            var command = new InteractCommand(ctx.Actor.Id, ctx.WorldState.InteractableTargetId);
            await commandDispatcher.DispatchAsync<InteractCommand, bool>(command);
        }
    }
}
