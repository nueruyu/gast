using UnityEngine;
using Gast.Domain.Characters;

namespace Gast.Domain.Combat
{
    /// <summary>
    /// Information about damage dealt to a character.
    /// Contains data needed for hit reactions and knockback calculations.
    /// </summary>
    public struct DamageInfo
    {
        public float Amount { get; set; }
        public Pose Origin { get; set; }
        public Vector3 KnockbackForce { get; set; }
        public CharacterId AttackerId { get; set; }

        public DamageInfo(float amount, Pose origin, Vector3 knockbackForce, CharacterId attackerId)
        {
            Amount = amount;
            Origin = origin;
            KnockbackForce = knockbackForce;
            AttackerId = attackerId;
        }
    }
}