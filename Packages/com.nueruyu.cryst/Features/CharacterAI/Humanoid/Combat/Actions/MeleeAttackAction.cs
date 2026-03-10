using System;
using System.Threading;
using Cryst.Domain.Characters.Facets;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Combat.Actions
{
    [Serializable]
    public class MeleeAttackActionSettings
    {
        [SerializeField] float alignmentTimeout = 1.0f;
        [SerializeField] float alignmentThreshold = 20.0f;
        [SerializeField] int postAttackDelayMs = 500;

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

        public async UniTask ExecuteAsync(ActorContext<CombatState> context, CancellationToken cancellationToken)
        {
            var actor = context.Actor;

            // 1. Step-in phase: Align facing direction naturally by moving toward target
            var timer = 0f;

            var navigator = actor.NavigationProvider;

            while (timer < settings.AlignmentTimeout && !cancellationToken.IsCancellationRequested)
            {
                var targetPos = context.WorldState.TargetPosition;
                var selfPos = actor.Body.Position;

                // Calculate direction to target
                var toTarget = targetPos - selfPos;
                toTarget.y = 0;
                if (toTarget.sqrMagnitude < 0.01f)
                    break; // Already on top of target
                toTarget.Normalize();

                var forward = actor.Body.Forward;

                // Check angle difference
                var angle = Vector3.Angle(forward, toTarget);
                if (angle <= settings.AlignmentThreshold)
                    break; // Aligned successfully

                navigator.SetDestination(selfPos + toTarget);
                // Move toward target to naturally rotate facing direction
                actor.Move(navigator.NextSteeringDirection);

                timer += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }

            // 2. Attack phase: Stop movement and execute attack
            navigator.Stop();
            actor.Move(Vector3.zero); // Stop movement input

            if (context.Character.Is(out AttackableCharacter attackable))
            {
                attackable.Attack();
            }

            await UniTask.Delay(settings.PostAttackDelayMs, cancellationToken: cancellationToken);
        }
    }
}
