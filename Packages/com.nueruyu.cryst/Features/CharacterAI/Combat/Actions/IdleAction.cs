using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System.Threading;

namespace Cryst.Features.CharacterAI.Combat.Actions
{
    public class IdleAction : IAction<ActorContext<CombatState>, CombatState>
    {
        public bool CanExecute(CombatState worldState)
        {
            return true;
        }

        public void Simulate(CombatState worldState)
        {
        }

        public UniTask ExecuteAsync(ActorContext<CombatState> context, CancellationToken cancellationToken)
        {
            return UniTask.WaitUntilCanceled(cancellationToken);
        }
    }
}
