using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gast.Features.AI.Combat.Actions
{
    [Serializable]
    public class StrafeAction : IAction<CombatState, AIContext<CombatState>>
    {
        const float MinDuration = 0.5f;
        const float MaxDuration = 2.5f;

        public bool CanExecute(CombatState worldState)
        {
            return worldState.HasTarget && worldState.IsInCombatRange;
        }

        public void Simulate(CombatState worldState)
        {
        }

        public async UniTask ExecuteAsync(AIContext<CombatState> ctx)
        {
            var actor = ctx.Actor;
            var navigator = actor.NavigationProvider;
            var duration = Random.Range(MinDuration, MaxDuration);
            var timer = 0f;

            var directionSign = Random.value > 0.5f ? 1f : -1f;

            var idealDist = Mathf.Max(0.5f, ctx.WorldState.AttackRange - 0.3f);

            try
            {
                await UniTask.Yield(PlayerLoopTiming.Update, ctx.CancellationToken);

                while (timer < duration && !ctx.CancellationToken.IsCancellationRequested)
                {
                    timer += Time.deltaTime;

                    var worldState = ctx.WorldState;
                    var targetPos = worldState.TargetPosition;
                    var targetFwd = worldState.TargetForward;
                    var selfPos = actor.Body.Position;

                    var selfToTarget = targetPos - selfPos;
                    selfToTarget.y = 0;

                    var toTargetDir = selfToTarget.normalized;
                    var currentDist = selfToTarget.magnitude;

                    if (toTargetDir.sqrMagnitude < 0.01f)
                    {
                        toTargetDir = actor.Body.Forward;
                    }

                    var dot = Vector3.Dot(targetFwd, -toTargetDir);

                    var tangent = Vector3.Cross(toTargetDir, Vector3.up);
                    var strafeDir = tangent * directionSign;

                    var gap = currentDist - idealDist;
                    var approachDir = Vector3.zero;

                    bool isInFront = dot > 0.86f;

                    if (Mathf.Abs(gap) > 0.1f)
                    {
                        float approachWeight;
                        if (gap > 0)
                        {
                            approachWeight = isInFront ? 0.1f : 0.8f;
                        }
                        else
                        {
                            approachWeight = 0.8f;
                        }

                        approachDir = toTargetDir * gap * approachWeight;
                    }

                    if (isInFront)
                    {
                        strafeDir *= 1.5f;
                    }

                    var finalMoveDir = (strafeDir + approachDir).normalized;
                    navigator.SetDestination(selfPos + finalMoveDir);

                    actor.Move(navigator.NextSteeringDirection);

                    await UniTask.Yield(PlayerLoopTiming.Update, ctx.CancellationToken);
                }
            }
            finally
            {
                navigator.Stop();
            }
        }
    }
}