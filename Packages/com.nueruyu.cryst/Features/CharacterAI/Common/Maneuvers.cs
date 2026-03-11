using System;
using Cryst.Domain.Characters;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Common
{
    public enum ManeuverDirection
    {
        Left = -1,
        Right = 1
    }

    public interface IManeuverStrategy
    {
        Vector3 CalculateMoveDirection(BaseCharacter actor, Vector3 targetPosition, Vector3 targetForward);
    }

    [Serializable]
    public class BackOffManeuverSettings
    {
        [SerializeField] float backOffDistance = 3.0f;
        public float BackOffDistance => backOffDistance;
    }

    [Serializable]
    public class StrafeManeuverSettings
    {
        [SerializeField] float idealDistanceOffset = 0.3f;
        [SerializeField] float inFrontDotThreshold = 0.86f;
        [SerializeField] float approachWeightWhenBehind = 0.8f;
        [SerializeField] float approachWeightWhenInFront = 0.1f;
        [SerializeField] float strafeMultiplierWhenInFront = 1.5f;
        [SerializeField] float minIdealDistance = 0.5f;
        public float IdealDistanceOffset => idealDistanceOffset;
        public float InFrontDotThreshold => inFrontDotThreshold;
        public float ApproachWeightWhenBehind => approachWeightWhenBehind;
        public float ApproachWeightWhenInFront => approachWeightWhenInFront;
        public float StrafeMultiplierWhenInFront => strafeMultiplierWhenInFront;
        public float MinIdealDistance => minIdealDistance;
    }

    [Serializable]
    public class StalkManeuverSettings
    {
        [SerializeField] float idealDistanceOffset = 0.5f;
        [SerializeField] float strafeWeight = 0.7f;
        [SerializeField] float approachWeight = 0.3f;
        public float IdealDistanceOffset => idealDistanceOffset;
        public float StrafeWeight => strafeWeight;
        public float ApproachWeight => approachWeight;
    }

    public readonly struct BackOffManeuver : IManeuverStrategy
    {
        public readonly BackOffManeuverSettings Settings;
        public BackOffManeuver(BackOffManeuverSettings settings) => Settings = settings;

        public Vector3 CalculateMoveDirection(BaseCharacter actor, Vector3 targetPosition, Vector3 targetForward)
        {
            var selfPos = actor.Body.Position;
            var targetToSelf = selfPos - targetPosition;
            targetToSelf.y = 0;
            return targetToSelf.normalized;
        }
    }

    public readonly struct StrafeManeuver : IManeuverStrategy
    {
        public readonly ManeuverDirection Direction;
        public readonly StrafeManeuverSettings Settings;
        public readonly float CombatRange;
        public StrafeManeuver(ManeuverDirection direction, StrafeManeuverSettings settings, float combatRange)
        {
            Direction = direction;
            Settings = settings;
            CombatRange = combatRange;
        }

        public Vector3 CalculateMoveDirection(BaseCharacter actor, Vector3 targetPosition, Vector3 targetForward)
        {
            var idealDist = Mathf.Max(Settings.MinIdealDistance, CombatRange - Settings.IdealDistanceOffset);
            var selfPos = actor.Body.Position;
            var selfToTarget = targetPosition - selfPos;
            selfToTarget.y = 0;
            var toTargetDir = selfToTarget.normalized;
            var currentDist = selfToTarget.magnitude;

            if (toTargetDir.sqrMagnitude < 0.01f) toTargetDir = actor.Body.Forward;

            var dot = Vector3.Dot(targetForward, -toTargetDir);
            var tangent = Vector3.Cross(toTargetDir, Vector3.up);
            var strafeDir = tangent * (int)Direction;
            var gap = currentDist - idealDist;
            var approachDir = Vector3.zero;

            var isInFront = dot > Settings.InFrontDotThreshold;

            if (Mathf.Abs(gap) > 0.1f)
            {
                var approachWeight = (gap > 0)
                    ? (isInFront ? Settings.ApproachWeightWhenInFront : Settings.ApproachWeightWhenBehind)
                    : Settings.ApproachWeightWhenBehind;
                approachDir = toTargetDir * gap * approachWeight;
            }
            if (isInFront)
            {
                strafeDir *= Settings.StrafeMultiplierWhenInFront;
            }

            return (strafeDir + approachDir).normalized;
        }
    }

    public readonly struct StalkManeuver : IManeuverStrategy
    {
        public readonly ManeuverDirection Direction;
        public readonly StalkManeuverSettings Settings;
        public readonly float AttackRange;
        public StalkManeuver(ManeuverDirection direction, StalkManeuverSettings settings, float attackRange)
        {
            Direction = direction;
            Settings = settings;
            AttackRange = attackRange;
        }

        public Vector3 CalculateMoveDirection(BaseCharacter actor, Vector3 targetPosition, Vector3 targetForward)
        {
            var idealDist = Mathf.Max(Settings.IdealDistanceOffset, AttackRange - Settings.IdealDistanceOffset);
            var selfPos = actor.Body.Position;
            var selfToTarget = targetPosition - selfPos;
            selfToTarget.y = 0;
            var toTargetDir = selfToTarget.normalized;
            var currentDist = selfToTarget.magnitude;

            var tangent = Vector3.Cross(toTargetDir, Vector3.up);
            var strafeDir = tangent * (int)Direction;

            var gap = currentDist - idealDist;
            var approachDir = toTargetDir * gap;

            return (strafeDir * Settings.StrafeWeight + approachDir * Settings.ApproachWeight).normalized;
        }
    }
}
