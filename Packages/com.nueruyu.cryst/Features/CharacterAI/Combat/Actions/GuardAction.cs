using Cysharp.Threading.Tasks;
using Cryst.Domain.Characters.Facets;
using Gast.Lib.AI;
using System;
using System.Threading;
using ActorContext_ = Cryst.Features.CharacterAI.ActorContext<Cryst.Features.CharacterAI.Combat.CombatState>;

namespace Cryst.Features.CharacterAI.Combat.Actions
{
    [Serializable]
    public class GuardAction : IAction<ActorContext_, CombatState>
    {
        public bool CanExecute(CombatState worldState)
        {
            return worldState.IsInAttackRange && !worldState.IsReadyToAttack && worldState.CanGuard;
        }

        public void Simulate(CombatState worldState)
        {
        }

        public async UniTask ExecuteAsync(ActorContext_ context, CancellationToken cancellationToken)
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
