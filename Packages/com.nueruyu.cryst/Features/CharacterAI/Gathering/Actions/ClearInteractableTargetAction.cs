using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System.Threading;

namespace Cryst.Features.CharacterAI.Gathering.Actions
{
    public class ClearInteractableTargetAction : IAction<ActorContext<GatheringState>, GatheringState>
    {
        public bool CanExecute(GatheringState worldState)
        {
            return worldState.HasInteractableTarget;
        }

        public void Simulate(GatheringState worldState)
        {
            worldState.HasInteractableTarget = false;
            worldState.IsInRangeToInteract = false;
        }

        public UniTask ExecuteAsync(ActorContext<GatheringState> context, CancellationToken cancellationToken)
        {
            context.Memory.InteractableTarget = null;
            return UniTask.CompletedTask;
        }
    }
}
