using Cysharp.Threading.Tasks;
using Gast.Lib.AI.Tasks;
using System;
using UnityEngine;

namespace Gast.Features.Npcs.Actions
{
    [Serializable]
    public class BackOffAction : PrimitiveTask<CombatWorldState, AIContext<CombatWorldState>>
    {
        public BackOffAction() : base("BackOff")
        {
        }

        protected override bool CanExecute(CombatWorldState worldState)
        {
            return worldState.IsInAttackRange && !worldState.IsReadyToAttack;
        }

        protected override void Simulate(CombatWorldState worldState)
        {
            worldState.DistanceToTarget += 2.0f;
        }

        protected override async UniTask ExecuteAsync(AIContext<CombatWorldState> ctx)
        {
            var actor = ctx.Actor;
            var worldState = ctx.WorldState;
            var navigator = actor.NavigationProvider;

            var selfPos = actor.Body.Position;
            var targetPos = worldState.TargetPosition;

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
                        actor.Move(moveDir);
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