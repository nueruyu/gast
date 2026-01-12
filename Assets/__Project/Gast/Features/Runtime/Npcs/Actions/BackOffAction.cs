using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;
using UnityEngine;

namespace Gast.Features.Npcs.Actions
{
    [Serializable]
    public class BackOffAction : PrimitiveTask<CombatWorldState>
    {
        public BackOffAction() : base("BackOff")
        {
        }

        protected override bool CanExecute(CombatWorldState state)
        {
            return state.IsInAttackRange && !state.IsReadyToAttack;
        }

        protected override void Simulate(ref CombatWorldState state)
        {
            state.DistanceToTarget += 2.0f;
        }

        protected override async UniTask ExecuteAsync(Context<CombatWorldState> ctx)
        {
            var currentState = ctx.CurrentState;
            var navigator = ctx.Character.NavigationProvider;

            var selfPos = ctx.Character.Body.Position;
            var targetPos = currentState.TargetPosition;

            var targetToSelf = selfPos - targetPos;
            targetToSelf.y = 0;
            var dirAway = targetToSelf.normalized;
            var dest = selfPos + dirAway * 3.0f;

            navigator.SetDestination(dest);

            try
            {
                var timer = 0f;
                while (timer < 1.5f && !ctx.CancellationToken.IsCancellationRequested)
                {
                    timer += Time.deltaTime;

                    var moveDir = navigator.NextSteeringDirection;
                    if (moveDir != Vector3.zero)
                    {
                        ctx.Character.Move(moveDir);
                    }

                    if (navigator.HasArrived)
                        break;

                    await UniTask.Yield(ctx.CancellationToken);
                }
            }
            finally
            {
                navigator.Stop();
            }
        }
    }
}