using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System.Threading;
using ActorContext_ = Cryst.Features.CharacterAI.ActorContext<Cryst.Features.CharacterAI.Gathering.GatheringState>;

namespace Cryst.Features.CharacterAI.Gathering.Actions
{
    public class ClearInteractableTargetAction : IAction<ActorContext_, GatheringState>
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

        public UniTask ExecuteAsync(ActorContext_ context, CancellationToken cancellationToken)
        {
            context.Memory.InteractableTarget = null;
            return UniTask.CompletedTask;
        }
    }
}
