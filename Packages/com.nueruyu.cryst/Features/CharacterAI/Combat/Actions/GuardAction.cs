using Cysharp.Threading.Tasks;
using Cryst.Domain.Characters.Facets;
using Gast.Lib.AI;
using System;

namespace Cryst.Features.CharacterAI.Combat.Actions
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
            if (!ctx.Actor.Character.Is(out GuardableCharacter guardable)) return;

            guardable.StartGuard();
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: ctx.CancellationToken);
            }
            finally
            {
                guardable.StopGuard();
            }
        }
    }
}
