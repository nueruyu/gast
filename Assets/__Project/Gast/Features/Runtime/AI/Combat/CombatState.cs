using Gast.Lib.AI;
using UnityEngine;

namespace Gast.Features.AI.Combat
{
    public class CombatState : IWorldState<CombatState>
    {
        public bool HasTarget { get; set; }
        public Vector3 TargetPosition { get; set; }
        public Vector3 TargetForward { get; set; }
        public float DistanceToTarget { get; set; }
        public bool IsReadyToAttack { get; set; }
        public float AttackRange { get; set; }
        public float CombatRange { get; set; }
        public bool IsInAttackRange => HasTarget && DistanceToTarget <= AttackRange;
        public bool IsInCombatRange => HasTarget && DistanceToTarget < CombatRange;

        public void CopyFrom(CombatState source)
        {
            HasTarget = source.HasTarget;
            TargetPosition = source.TargetPosition;
            TargetForward = source.TargetForward;
            DistanceToTarget = source.DistanceToTarget;
            IsReadyToAttack = source.IsReadyToAttack;
            AttackRange = source.AttackRange;
            CombatRange = source.CombatRange;
        }

        public override string ToString()
        {
            return $"Combat[Dist:{DistanceToTarget:F1}, InRange:{IsInAttackRange}, Ready:{IsReadyToAttack}]";
        }
    }
}