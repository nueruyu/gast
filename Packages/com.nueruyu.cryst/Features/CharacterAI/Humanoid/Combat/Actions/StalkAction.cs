using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Core.Values;
using Gast.Lib.AI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cryst.Features.CharacterAI.Humanoid.Combat.Actions
{
    public class StalkActionSettings
    {
        public FloatRange Duration { get; set; } = new(0.2f, 0.6f);
        public float IdealDistanceOffset { get; set; } = 0.5f;
        public float StrafeWeight { get; set; } = 0.7f;
        public float ApproachWeight { get; set; } = 0.3f;
        public float MoveDistance { get; set; } = 0.5f;
    }

    /// <summary>
    /// An action where the AI moves around the target for a short period of time to time an attack.
    /// </summary>
    public class StalkAction : IAction<ActorContext<CombatState>, CombatState>
    {
        readonly StalkActionSettings settings;

        public StalkAction(StalkActionSettings settings = null)
        {
            this.settings = settings ?? new StalkActionSettings();
        }

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
            var duration = settings.Duration.Sample();
            var timer = 0f;

            var directionSign = Random.value > 0.5f ? 1f : -1f;
            var idealDist = Mathf.Max(settings.IdealDistanceOffset, context.WorldState.AttackRange - settings.IdealDistanceOffset);

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
                    var finalMoveDir = (strafeDir * settings.StrafeWeight + approachDir * settings.ApproachWeight).normalized;

                    // Set a destination a short distance away to create small movements
                    navigator.SetDestination(selfPos + finalMoveDir * settings.MoveDistance);
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
