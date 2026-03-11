using System;
using System.Threading;
using Cryst.Domain.Characters.Facets;
using Cryst.Features.CharacterAI.Common;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Combat.Actions
{
    [Serializable]
    public class MeleeAttackActionSettings
    {
        [SerializeField]
        float alignmentTimeout = 1.0f;

        [SerializeField]
        float alignmentThreshold = 20.0f;

        [SerializeField]
        int postAttackDelayMs = 500;

        public float AlignmentTimeout => alignmentTimeout;
        public float AlignmentThreshold => alignmentThreshold;
        public int PostAttackDelayMs => postAttackDelayMs;
    }

    public class MeleeAttackAction : IAction<ActorContext<CombatState>, CombatState>
    {
        readonly MeleeAttackActionSettings settings;

        public MeleeAttackAction(MeleeAttackActionSettings settings = null)
        {
            this.settings = settings ?? new MeleeAttackActionSettings();
        }

        public bool CanExecute(CombatState worldState)
        {
            return worldState.IsInAttackRange && worldState.IsReadyToAttack;
        }

        public void Simulate(CombatState worldState)
        {
            worldState.IsReadyToAttack = false;
        }

        public async UniTask ExecuteAsync(
            ActorContext<CombatState> context,
            CancellationToken cancellationToken)
        {
            // 1. Step-in phase: Align facing direction naturally by moving toward target
            await context.Actor.FaceTowardsAsync(
                static state => state.TargetPosition,
                context.WorldState,
                settings.AlignmentThreshold,
                settings.AlignmentTimeout,
                cancellationToken);

            // 2. Attack phase: Execute attack
            if (context.Character.Is(out AttackableCharacter attackable))
                attackable.Attack();

            await UniTask.Delay(settings.PostAttackDelayMs, cancellationToken: cancellationToken);
        }
    }
}