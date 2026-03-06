using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;
using System.Threading;
using UnityEngine;
using ActorContext_ = Cryst.Features.CharacterAI.ActorContext<Cryst.Features.CharacterAI.Combat.CombatState>;

namespace Cryst.Features.CharacterAI.Combat.Actions
{
    [Serializable]
    public class BackOffAction : IAction<ActorContext_, CombatState>
    {
        public bool CanExecute(CombatState worldState)
        {
            return worldState.IsInAttackRange && !worldState.IsReadyToAttack;
        }

        public void Simulate(CombatState worldState)
        {
            worldState.DistanceToTarget += 2.0f;
        }

        public async UniTask ExecuteAsync(ActorContext_ context, CancellationToken cancellationToken)
        {
            var actor = context.Actor;
            var worldState = context.WorldState;
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
                while (timer < 1.5f && !cancellationToken.IsCancellationRequested)
                {
                    timer += Time.deltaTime;

                    var moveDir = navigator.NextSteeringDirection;
                    if (moveDir != Vector3.zero)
                    {
                        actor.Move(moveDir);
                    }

                    if (navigator.HasArrived)
                        break;

                    await UniTask.Yield(cancellationToken);
                }
            }
            finally
            {
                navigator.Stop();
            }
        }
    }
}
