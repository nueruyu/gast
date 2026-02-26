using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Combat.Actions
{
    [Serializable]
    public class MeleeAttackAction : IAction<CombatState, AIContext<CombatState>>
    {
        public bool CanExecute(CombatState worldState)
        {
            return worldState.IsInAttackRange && worldState.IsReadyToAttack;
        }

        public void Simulate(CombatState worldState)
        {
            worldState.IsReadyToAttack = false;
        }

        public async UniTask ExecuteAsync(AIContext<CombatState> ctx)
        {
            var actor = ctx.Actor;

            // 1. Step-in phase: Align facing direction naturally by moving toward target
            const float alignmentTimeout = 1.0f;
            const float alignmentThreshold = 20f; // degrees
            var timer = 0f;

            var navigator = actor.NavigationProvider;

            while (timer < alignmentTimeout && !ctx.CancellationToken.IsCancellationRequested)
            {
                var targetPos = ctx.WorldState.TargetPosition;
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
                if (angle <= alignmentThreshold)
                    break; // Aligned successfully

                navigator.SetDestination(selfPos + toTarget);
                // Move toward target to naturally rotate facing direction
                actor.Move(navigator.NextSteeringDirection);

                timer += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, ctx.CancellationToken);
            }

            // 2. Attack phase: Stop movement and execute attack
            navigator.Stop();
            actor.Move(Vector3.zero); // Stop movement input

            actor.Attack();

            await UniTask.Delay(500, cancellationToken: ctx.CancellationToken);
        }
    }
}