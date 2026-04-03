using Gast.Core.Observables;
using Gast.Core.Stats;
using Gast.Domain.Equipment;
using UnityEngine;

namespace Cryst.Domain.Characters
{
    public class CharacterStatus : IEquipmentBonusApplicable
    {
        readonly Live<float> health;
        readonly Live<float> maxHealth;
        readonly Live<float> hunger;
        readonly Live<float> maxHunger;
        readonly float baseMaxHealth;

        public static readonly TypedKey<float> MaxHpKey = new();

        public ILive<float> Health => health;
        public ILive<float> MaxHealth => maxHealth;
        public ILive<float> Hunger => hunger;
        public ILive<float> MaxHunger => maxHunger;
        public ILive<bool> IsAlive { get; }

        public CharacterStatus(float maxHealth, float maxHunger)
        {
            baseMaxHealth = maxHealth;
            health = new Live<float>(maxHealth);
            this.maxHealth = new Live<float>(maxHealth);
            hunger = new Live<float>(maxHunger);
            this.maxHunger = new Live<float>(maxHunger);

            IsAlive = health.Select(h => h > 0);
        }

        public void SetHealth(float newHealth)
        {
            health.Value = Mathf.Clamp(newHealth, 0, maxHealth.Value);
        }

        public void SetHunger(float newHunger)
        {
            hunger.Value = Mathf.Clamp(newHunger, 0, maxHunger.Value);
        }

        public void ApplyEquipmentBonuses(StatBuilder stats)
        {
            var newMaxHealth = baseMaxHealth + stats.Get(MaxHpKey);
            maxHealth.Value = newMaxHealth;
            health.Value = Mathf.Clamp(health.Value, 0, newMaxHealth);
        }
    }
}
