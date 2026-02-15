using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace Cryst.Modules.CharacterAI.Combat.Actions
{
    public class IdleAction : IAction<CombatState, AIContext<CombatState>>
    {
        public bool CanExecute(CombatState worldState)
        {
            return true;
        }

        public void Simulate(CombatState worldState)
        {
        }

        public UniTask ExecuteAsync(AIContext<CombatState> ctx)
        {
            return UniTask.WaitUntilCanceled(ctx.CancellationToken);
        }
    }
}