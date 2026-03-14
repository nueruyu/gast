using System;
using System.Threading;
using Cryst.Domain.Characters.Facets;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Combat.Actions
{
    [Serializable]
    public class GuardActionSettings
    {
        [SerializeField] float duration = 1.5f;

        public float Duration => duration;
    }

    public class GuardAction : IAction<ActorContext<CombatState>, CombatState>
    {
        readonly GuardActionSettings settings;

        public GuardAction(GuardActionSettings settings = null)
        {
            this.settings = settings ?? new GuardActionSettings();
        }

        public bool IsAvailable(CombatState worldState)
        {
            return worldState.HasTarget && worldState.CanGuard;
        }

        public void Simulate(CombatState worldState)
        {
        }

        public async UniTask ExecuteAsync(
            ActorContext<CombatState> context,
            CancellationToken cancellationToken)
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