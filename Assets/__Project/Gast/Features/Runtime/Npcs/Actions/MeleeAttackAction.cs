using Cysharp.Threading.Tasks;
using DescrioGames.Lib.AI;
using System;
using UnityEngine;

namespace DescrioGames.Features.Npcs.Actions
{
    [Serializable]
    public class MeleeAttackAction : PrimitiveTask<CombatWorldState>
    {
        public MeleeAttackAction() : base("MeleeAttack")
        {
        }

        protected override bool CheckCondition(CombatWorldState state)
        {
            return state.IsInAttackRange && state.IsReadyToAttack;
        }

        protected override void ApplyEffect(ref CombatWorldState state, ISimulationContext context)
        {
            state.IsReadyToAttack = false;
        }

        protected override async UniTask ExecuteAsync(Context<CombatWorldState> ctx)
        {
            // 1. Step-in phase: Align facing direction naturally by moving toward target
            const float alignmentTimeout = 1.0f;
            const float alignmentThreshold = 20f; // degrees
            var timer = 0f;

            var navigator = ctx.Character.NavigationProvider;

            while (timer < alignmentTimeout && !ctx.Token.IsCancellationRequested)
            {
                var targetPos = ctx.CurrentState.TargetPosition;
                var selfPos = ctx.Character.Body.Position;

                // Calculate direction to target
                var toTarget = targetPos - selfPos;
                toTarget.y = 0;
                if (toTarget.sqrMagnitude < 0.01f) break; // Already on top of target
                toTarget.Normalize();

                var forward = ctx.Character.Body.Forward;

                // Check angle difference
                var angle = Vector3.Angle(forward, toTarget);
                if (angle <= alignmentThreshold) break; // Aligned successfully

                // Move toward target to naturally rotate facing direction
                ctx.Character.Move(toTarget);

                timer += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, ctx.Token);
            }

            // 2. Attack phase: Stop movement and execute attack
            navigator.Stop();
            ctx.Character.Move(Vector3.zero); // Stop movement input

            ctx.Character.Attack();

            await UniTask.Delay(500, cancellationToken: ctx.Token);
        }
    }
}