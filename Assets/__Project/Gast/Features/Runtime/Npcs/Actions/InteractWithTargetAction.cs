using Cysharp.Threading.Tasks;
using Gast.Api.Interactions;
using Gast.Core.Commands;
using Gast.Lib.AI;

namespace Gast.Features.Npcs.Actions
{
    public class InteractWithTargetAction : PrimitiveTask<StrategicWorldState>
    {
        readonly ICommandDispatcher commandDispatcher;

        public InteractWithTargetAction(ICommandDispatcher commandDispatcher) : base("InteractWithTargetAction")
        {
            this.commandDispatcher = commandDispatcher;
        }

        protected override bool CanExecute(StrategicWorldState state)
        {
            return state.HasInteractableTarget && state.IsInRangeToInteract;
        }

        protected override void Simulate(ref StrategicWorldState state)
        {
            state.HasInteractableTarget = false;
            state.IsInRangeToInteract = false;
        }

        protected override async UniTask ExecuteAsync(Context<StrategicWorldState> ctx)
        {
            var command = new InteractCommand(ctx.Character.Id, ctx.CurrentState.InteractableTargetId);
            await commandDispatcher.DispatchAsync<InteractCommand, bool>(command);
        }
    }
}