using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System.Threading;
using ActorContext_ = Cryst.Features.CharacterAI.ActorContext<Cryst.Features.CharacterAI.Strategic.StrategicState>;

namespace Cryst.Features.CharacterAI.Strategic.Actions
{
    public class ClearTargetAction : IAction<ActorContext_, StrategicState>
    {
        public bool CanExecute(StrategicState worldState)
        {
            return true;
        }

        public void Simulate(StrategicState worldState)
        {
            // This is a cognitive action that clears memory,
            // it does not change the predictable future state of the world for planning.
        }

        public UniTask ExecuteAsync(ActorContext_ context, CancellationToken cancellationToken)
        {
            context.Memory.CombatTarget = null;
            context.Memory.CurrentObjective = null;
            context.Memory.InteractableTarget = null;
            return UniTask.CompletedTask;
        }
    }
}
