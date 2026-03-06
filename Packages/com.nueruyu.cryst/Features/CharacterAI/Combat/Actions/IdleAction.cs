using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System.Threading;
using ActorContext_ = Cryst.Features.CharacterAI.ActorContext<Cryst.Features.CharacterAI.Combat.CombatState>;

namespace Cryst.Features.CharacterAI.Combat.Actions
{
    public class IdleAction : IAction<ActorContext_, CombatState>
    {
        public bool CanExecute(CombatState worldState)
        {
            return true;
        }

        public void Simulate(CombatState worldState)
        {
        }

        public UniTask ExecuteAsync(ActorContext_ context, CancellationToken cancellationToken)
        {
            return UniTask.WaitUntilCanceled(cancellationToken);
        }
    }
}
