using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gast.Features.Npcs.Actions
{
    [Serializable]
    public class StrafeAction : PrimitiveTask<CombatWorldState, AIContext<CombatWorldState>>
    {
        const float MinDuration = 1.0f;
        const float MaxDuration = 3.0f;

        public StrafeAction() : base("Strafe")
        {
        }

        protected override bool CanExecute(CombatWorldState worldState)
        {
            return worldState.HasTarget && worldState.IsInCombatRange;
        }

        protected override void Simulate(CombatWorldState worldState)
        {
        }

        protected override async UniTask ExecuteAsync(AIContext<CombatWorldState> ctx)
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

                    actor.Move(finalMoveDir);

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
