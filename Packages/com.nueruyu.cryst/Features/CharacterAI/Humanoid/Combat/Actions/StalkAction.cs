using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cryst.Features.CharacterAI.Humanoid.Combat.Actions
{
    /// <summary>
    /// An action where the AI moves around the target for a short period of time to time an attack.
    /// </summary>
    [Serializable]
    public class StalkAction : IAction<ActorContext<CombatState>, CombatState>
    {
        public bool CanExecute(CombatState worldState)
        {
            return worldState.HasTarget && worldState.IsInCombatRange;
        }

        public void Simulate(CombatState worldState)
        {
            // This action does not significantly change the world state in simulation.
        }

        public async UniTask ExecuteAsync(ActorContext<CombatState> context, CancellationToken cancellationToken)
        {
            var actor = context.Actor;
            var navigator = actor.NavigationProvider;
            var duration = Random.Range(0.2f, 0.6f);
            var timer = 0f;

            var directionSign = Random.value > 0.5f ? 1f : -1f;
            var idealDist = Mathf.Max(0.5f, context.WorldState.AttackRange - 0.5f);

            try
            {
                while (timer < duration && !cancellationToken.IsCancellationRequested)
                {
                    timer += Time.deltaTime;

                    var worldState = context.WorldState;
                    if (!worldState.HasTarget)
                        break;

                    var targetPos = worldState.TargetPosition;
                    var selfPos = actor.Body.Position;

                    var selfToTarget = targetPos - selfPos;
                    selfToTarget.y = 0;

                    var toTargetDir = selfToTarget.normalized;
                    var currentDist = selfToTarget.magnitude;

                    // Calculate strafe direction
                    var tangent = Vector3.Cross(toTargetDir, Vector3.up);
                    var strafeDir = tangent * directionSign;

                    // Calculate direction for distance adjustment
                    var gap = currentDist - idealDist;
                    var approachDir = toTargetDir * gap;

                    // Combine movements to create a circling motion while adjusting distance
                    var finalMoveDir = (strafeDir * 0.7f + approachDir * 0.3f).normalized;

                    // Set a destination a short distance away to create small movements
                    navigator.SetDestination(selfPos + finalMoveDir * 0.5f);
                    actor.Move(navigator.NextSteeringDirection);

                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }
            }
            finally
            {
                navigator.Stop();
                actor.Move(Vector3.zero);
            }
        }
    }
}
