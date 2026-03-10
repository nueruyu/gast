using System;
using System.Threading;
using Cryst.Domain.Characters.Facets;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Humanoid.Combat.Actions
{
    public class GuardActionSettings
    {
        public float Duration { get; set; } = 1.5f;
    }

    public class GuardAction : IAction<ActorContext<CombatState>, CombatState>
    {
        readonly GuardActionSettings settings;

        public GuardAction(GuardActionSettings settings = null)
        {
            this.settings = settings ?? new GuardActionSettings();
        }

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
                await UniTask.Delay(TimeSpan.FromSeconds(settings.Duration), cancellationToken: cancellationToken);
            }
            finally
            {
                guardable.StopGuard();
            }
        }
    }
}
