using Gast.Lib.AI;
using UnityEngine;

namespace Cryst.Modules.CharacterAI.Combat
{
    public class CombatState : IWorldState<CombatState>
    {
        public bool HasTarget { get; set; }
        public Vector3 TargetPosition { get; set; }
        public Vector3 TargetForward { get; set; }
        public float DistanceToTarget { get; set; }
        public bool IsReadyToAttack { get; set; }
        public bool CanGuard { get; set; }
        public float SelfHealthRatio { get; set; }
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
            CanGuard = source.CanGuard;
            SelfHealthRatio = source.SelfHealthRatio;
            AttackRange = source.AttackRange;
            CombatRange = source.CombatRange;
        }

        public override string ToString()
        {
            return $"Combat[Dist:{DistanceToTarget:F1}, InRange:{IsInAttackRange}, Ready:{IsReadyToAttack}, HP:{SelfHealthRatio:P0}]";
        }
    }
}