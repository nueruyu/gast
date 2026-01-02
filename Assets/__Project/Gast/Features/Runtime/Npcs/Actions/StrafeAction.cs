using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gast.Features.Npcs.Actions
{
    [Serializable]
    public class StrafeAction : PrimitiveTask<CombatWorldState>
    {
        const float MinDuration = 1.0f;
        const float MaxDuration = 3.0f;

        public StrafeAction() : base("Strafe")
        {
        }

        protected override bool CheckCondition(CombatWorldState state)
        {
            return state.HasTarget && state.IsInCombatRange;
        }

        protected override void ApplyEffect(ref CombatWorldState state, ISimulationContext context)
        {
        }

        protected override async UniTask ExecuteAsync(Context<CombatWorldState> ctx)
        {
            var navigator = ctx.Character.NavigationProvider;
            var duration = Random.Range(MinDuration, MaxDuration);
            var timer = 0f;

            var directionSign = Random.value > 0.5f ? 1f : -1f;

            var idealDist = Mathf.Max(0.5f, ctx.CurrentState.AttackRange - 0.3f);

            try
            {
                await UniTask.Yield(PlayerLoopTiming.Update, ctx.Token);

                while (timer < duration && !ctx.Token.IsCancellationRequested)
                {
                    timer += Time.deltaTime;

                    var currentState = ctx.CurrentState;
                    var targetPos = currentState.TargetPosition;
                    var targetFwd = currentState.TargetForward;
                    var selfPos = ctx.Character.Body.Position;

                    var selfToTarget = targetPos - selfPos;
                    selfToTarget.y = 0;

                    var toTargetDir = selfToTarget.normalized;
                    var currentDist = selfToTarget.magnitude;

                    if (toTargetDir.sqrMagnitude < 0.01f)
                    {
                        toTargetDir = ctx.Character.Body.Forward;
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

                    ctx.Character.Move(finalMoveDir);

                    await UniTask.Yield(PlayerLoopTiming.Update, ctx.Token);
                }
            }
            finally
            {
                navigator.Stop();
            }
        }
    }
}