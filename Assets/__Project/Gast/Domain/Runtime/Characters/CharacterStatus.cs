using UnityEngine;
using Gast.Core.Observables;

namespace Gast.Domain.Characters
{
    /// <summary>
    /// Manages dynamic character stats (Health, IsAlive).
    /// Uses observable Live values for reactive state updates.
    /// </summary>
    public class CharacterStatus
    {
        readonly Live<float> health;
        readonly Live<bool> isAlive;
        readonly float maxHealth;
        readonly Faction faction;

        public ILive<float> Health => health;
        public ILive<bool> IsAlive => isAlive;
        public float MaxHealth => maxHealth;
        public Faction Faction => faction;

        public CharacterStatus(float maxHealth, Faction faction)
        {
            this.maxHealth = maxHealth;
            this.faction = faction;
            this.health = new Live<float>(maxHealth);
            this.isAlive = new Live<bool>(true);
        }

        /// <summary>
        /// Apply damage to the character's health.
        /// If health reaches 0, sets IsAlive to false.
        /// </summary>
        public void ApplyDamage(float amount)
        {
            if (!isAlive.Value) return;

            var current = health.Value;
            var next = Mathf.Max(0f, current - amount);
            health.Value = next;

            if (next <= 0f)
            {
                isAlive.Value = false;
            }
        }
    }
}
