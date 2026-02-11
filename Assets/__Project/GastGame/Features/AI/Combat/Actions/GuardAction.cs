using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;

namespace GastGame.AI.Combat.Actions
{
    [Serializable]
    public class GuardAction : IAction<CombatState, AIContext<CombatState>>
    {
        public bool CanExecute(CombatState worldState)
        {
            return worldState.IsInAttackRange && !worldState.IsReadyToAttack && worldState.CanGuard;
        }

        public void Simulate(CombatState worldState)
        {
        }

        public async UniTask ExecuteAsync(AIContext<CombatState> ctx)
        {
            var actor = ctx.Actor;
            actor.StartGuard();
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: ctx.CancellationToken);
            }
            finally
            {
                actor.StopGuard();
            }
        }
    }
}
