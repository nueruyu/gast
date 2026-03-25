using Cryst.Domain.Characters;
using Cryst.Domain.Characters.Facets;
using Gast.Lib.AI;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Combat
{
    public class CombatState : IWorldState<CombatState>
    {
        public bool IsActive { get; private set; }
        public Vector3 TargetPosition { get; private set; }
        public Vector3 TargetForward { get; private set; }
        public bool CanGuard { get; private set; }
        public float SelfHealthRatio { get; private set; }
        public float AttackRange { get; private set; }
        public float CombatRange { get; private set; }
        public float DistanceToTarget { get; private set; }
        public bool IsReadyToAttack { get; private set; }
        public bool IsInAttackRange => DistanceToTarget <= AttackRange;
        public bool IsInCombatRange => DistanceToTarget < CombatRange;

        public void WriteTo(ref CombatState dest)
        {
            dest ??= new();
            dest.IsActive = IsActive;
            dest.TargetPosition = TargetPosition;
            dest.TargetForward = TargetForward;
            dest.DistanceToTarget = DistanceToTarget;
            dest.IsReadyToAttack = IsReadyToAttack;
            dest.CanGuard = CanGuard;
            dest.SelfHealthRatio = SelfHealthRatio;
            dest.AttackRange = AttackRange;
            dest.CombatRange = CombatRange;
        }

        public void SetDistanceToTarget(float distance)
        {
            DistanceToTarget = distance;
        }

        public void SetReadyToAttack(bool isReady)
        {
            IsReadyToAttack = isReady;
        }

        public void Update(
            bool isActive,
            BaseCharacter combatTarget,
            Vector3 actorPosition,
            bool isReadyToAttack,
            bool canGuard,
            float selfHealthRatio)
        {
            IsActive = isActive;
            AttackRange = 1.5f;
            CombatRange = 4.5f;

            if (combatTarget != null)
            {
                TargetPosition = combatTarget.Body.Position;
                TargetForward = combatTarget.Body.Forward;
                DistanceToTarget = Vector3.Distance(actorPosition, combatTarget.Body.Position);
            }
            else
            {
                DistanceToTarget = float.PositiveInfinity;
            }

            IsReadyToAttack = isReadyToAttack;
            CanGuard = canGuard;
            SelfHealthRatio = selfHealthRatio;
        }

        public override string ToString()
        {
            return
                $"Combat[Dist:{DistanceToTarget:F1}, InRange:{IsInAttackRange}, Ready:{IsReadyToAttack}, HP:{SelfHealthRatio:P0}]";
        }
    }
}
