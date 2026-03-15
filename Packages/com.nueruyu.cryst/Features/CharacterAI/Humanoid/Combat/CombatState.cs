using Gast.Lib.AI;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Combat
{
    public class CombatState : IWorldState<CombatState>
    {
        public AIMode CurrentMode { get; set; }
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

        public void WriteTo(ref CombatState dest)
        {
            dest ??= new();
            dest.CurrentMode = CurrentMode;
            dest.HasTarget = HasTarget;
            dest.TargetPosition = TargetPosition;
            dest.TargetForward = TargetForward;
            dest.DistanceToTarget = DistanceToTarget;
            dest.IsReadyToAttack = IsReadyToAttack;
            dest.CanGuard = CanGuard;
            dest.SelfHealthRatio = SelfHealthRatio;
            dest.AttackRange = AttackRange;
            dest.CombatRange = CombatRange;
        }

        public override string ToString()
        {
            return
                $"Combat[Dist:{DistanceToTarget:F1}, InRange:{IsInAttackRange}, Ready:{IsReadyToAttack}, HP:{SelfHealthRatio:P0}]";
        }
    }
}