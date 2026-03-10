using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Combat.Actions
{
    public class BackOffActionSettings
    {
        public float BackOffDistance { get; set; } = 3.0f;
        public float Duration { get; set; } = 1.5f;
    }

    public class BackOffAction : IAction<ActorContext<CombatState>, CombatState>
    {
        readonly BackOffActionSettings settings;

        public BackOffAction(BackOffActionSettings settings = null)
        {
            this.settings = settings ?? new BackOffActionSettings();
        }

        public bool CanExecute(CombatState worldState)
        {
            return worldState.IsInAttackRange && !worldState.IsReadyToAttack;
        }

        public void Simulate(CombatState worldState)
        {
            worldState.DistanceToTarget += 2.0f;
        }

        public async UniTask ExecuteAsync(ActorContext<CombatState> context, CancellationToken cancellationToken)
        {
            var actor = context.Actor;
            var worldState = context.WorldState;
            var navigator = actor.NavigationProvider;

            var selfPos = actor.Body.Position;
            var targetPos = worldState.TargetPosition;

            var targetToSelf = selfPos - targetPos;
            targetToSelf.y = 0;
            var dirAway = targetToSelf.normalized;
            var dest = selfPos + dirAway * settings.BackOffDistance;

            navigator.SetDestination(dest);

            try
            {
                var timer = 0f;
                while (timer < settings.Duration && !cancellationToken.IsCancellationRequested)
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
