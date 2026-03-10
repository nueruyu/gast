using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Core.Values;
using Gast.Lib.AI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cryst.Features.CharacterAI.Humanoid.Combat.Actions
{
    [Serializable]
    public class StrafeActionSettings
    {
        [SerializeField] FloatRange duration = new(0.5f, 2.5f);
        [SerializeField] float idealDistanceOffset = 0.3f;
        [SerializeField] float inFrontDotThreshold = 0.86f;
        [SerializeField] float approachWeightWhenBehind = 0.8f;
        [SerializeField] float approachWeightWhenInFront = 0.1f;
        [SerializeField] float strafeMultiplierWhenInFront = 1.5f;

        public FloatRange Duration => duration;
        public float IdealDistanceOffset => idealDistanceOffset;
        public float InFrontDotThreshold => inFrontDotThreshold;
        public float ApproachWeightWhenBehind => approachWeightWhenBehind;
        public float ApproachWeightWhenInFront => approachWeightWhenInFront;
        public float StrafeMultiplierWhenInFront => strafeMultiplierWhenInFront;
    }

    public class StrafeAction : IAction<ActorContext<CombatState>, CombatState>
    {
        readonly StrafeActionSettings settings;

        public StrafeAction(StrafeActionSettings settings = null)
        {
            this.settings = settings ?? new StrafeActionSettings();
        }

        public bool CanExecute(CombatState worldState)
        {
            return worldState.HasTarget && worldState.IsInCombatRange;
        }

        public void Simulate(CombatState worldState)
        {
        }

        public async UniTask ExecuteAsync(ActorContext<CombatState> context, CancellationToken cancellationToken)
        {
            var actor = context.Actor;
            var navigator = actor.NavigationProvider;
            var duration = settings.Duration.Sample();
            var timer = 0f;

            var directionSign = Random.value > 0.5f ? 1f : -1f;

            var idealDist = Mathf.Max(0.5f, context.WorldState.AttackRange - settings.IdealDistanceOffset);

            try
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

                while (timer < duration && !cancellationToken.IsCancellationRequested)
                {
                    timer += Time.deltaTime;

                    var worldState = context.WorldState;
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

                    bool isInFront = dot > settings.InFrontDotThreshold;

                    if (Mathf.Abs(gap) > 0.1f)
                    {
                        float approachWeight;
                        if (gap > 0)
                        {
                            approachWeight = isInFront ? settings.ApproachWeightWhenInFront : settings.ApproachWeightWhenBehind;
                        }
                        else
                        {
                            approachWeight = settings.ApproachWeightWhenBehind;
                        }

                        approachDir = toTargetDir * gap * approachWeight;
                    }

                    if (isInFront)
                    {
                        strafeDir *= settings.StrafeMultiplierWhenInFront;
                    }

                    var finalMoveDir = (strafeDir + approachDir).normalized;
                    navigator.SetDestination(selfPos + finalMoveDir);

                    actor.Move(navigator.NextSteeringDirection);

                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }
            }
            finally
            {
                navigator.Stop();
            }
        }
    }
}
