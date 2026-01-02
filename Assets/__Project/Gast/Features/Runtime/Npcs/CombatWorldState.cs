using UnityEngine;

namespace DescrioGames.Features.Npcs
{
    /// <summary>
    /// World state for melee combat behavior.
    /// Immutable struct containing all state needed for melee combat planning and execution.
    /// </summary>
    public struct CombatWorldState
    {
        // Target information
        public bool HasTarget { get; set; }

        public Vector3 TargetPosition { get; set; }
        public Vector3 TargetForward { get; set; }

        // Distance and attack readiness
        public float DistanceToTarget { get; set; }

        public bool IsReadyToAttack { get; set; } // Not in cooldown

        // Attack settings (defined by Brain or retrieved from Character)
        public float AttackRange { get; set; } // e.g. 1.5f for melee

        public float CombatRange { get; set; } // e.g. 4.5f for tactical positioning

        // Helper properties
        public bool IsInAttackRange => HasTarget && DistanceToTarget <= AttackRange;

        public bool IsInCombatRange => HasTarget && DistanceToTarget < CombatRange;

        // Immutable update helpers
        public CombatWorldState WithTarget(Vector3 pos, float dist)
        {
            var state = this;
            state.HasTarget = true;
            state.TargetPosition = pos;
            state.DistanceToTarget = dist;
            return state;
        }

        public override string ToString()
        {
            return $"Combat[Dist:{DistanceToTarget:F1}, InRange:{IsInAttackRange}, Ready:{IsReadyToAttack}]";
        }
    }
}