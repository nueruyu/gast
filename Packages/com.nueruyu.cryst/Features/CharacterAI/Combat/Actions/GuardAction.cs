using Cysharp.Threading.Tasks;
using Cryst.Domain.Characters.Facets;
using Gast.Lib.AI;
using System;
using System.Threading;

namespace Cryst.Features.CharacterAI.Combat.Actions
{
    [Serializable]
    public class GuardAction : IAction<ActorContext<CombatState>, CombatState>
    {
        public bool CanExecute(CombatState worldState)
        {
            return worldState.IsInAttackRange && !worldState.IsReadyToAttack && worldState.CanGuard;
        }

        public void Simulate(CombatState worldState)
        {
        }

        public async UniTask ExecuteAsync(ActorContext<CombatState> context, CancellationToken cancellationToken)
        {
            if (!context.Character.Is(out GuardableCharacter guardable)) return;

            guardable.StartGuard();
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: cancellationToken);
            }
            finally
            {
                guardable.StopGuard();
            }
        }
    }
}
