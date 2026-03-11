using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Core.Values;
using Gast.Lib.AI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cryst.Features.CharacterAI.Humanoid.Combat.Actions
{
    [Serializable]
    public class PostAttackManeuverActionSettings
    {
        [SerializeField]
        Rate guardChance = 0.3f;

        [SerializeField]
        Rate strafeChance = 0.4f;

        public Rate GuardChance => guardChance;
        public Rate StrafeChance => strafeChance;
    }

    public class PostAttackManeuverAction : IAction<ActorContext<CombatState>, CombatState>
    {
        readonly BackOffAction backOffAction;
        readonly GuardAction guardAction;
        readonly PostAttackManeuverActionSettings settings;
        readonly StrafeAction strafeAction;

        public PostAttackManeuverAction(
            GuardAction guardAction,
            StrafeAction strafeAction,
            BackOffAction backOffAction,
            PostAttackManeuverActionSettings settings = null)
        {
            this.guardAction = guardAction;
            this.strafeAction = strafeAction;
            this.backOffAction = backOffAction;
            this.settings = settings ?? new PostAttackManeuverActionSettings();
        }

        public bool CanExecute(CombatState worldState)
        {
            return worldState.IsInAttackRange;
        }

        public void Simulate(CombatState worldState)
        {
            backOffAction.Simulate(worldState);
        }

        public async UniTask ExecuteAsync(
            ActorContext<CombatState> context,
            CancellationToken cancellationToken)
        {
            var rand = Random.value;

            if (context.WorldState.CanGuard && rand < settings.GuardChance.Value)
                await guardAction.ExecuteAsync(context, cancellationToken);
            else if (rand < settings.GuardChance.Value + settings.StrafeChance.Value)
                await strafeAction.ExecuteAsync(context, cancellationToken);
            else
                await backOffAction.ExecuteAsync(context, cancellationToken);
        }
    }
}