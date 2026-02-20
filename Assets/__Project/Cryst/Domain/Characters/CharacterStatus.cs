using Gast.Core.Observables;
using Gast.Domain.Characters;
using UnityEngine;

namespace Cryst.Domain.Characters
{
    public class CharacterStatus : IHasHealthStatus
    {
        readonly Live<float> health;
        readonly Live<float> maxHealth;

        public ILive<float> Health => health;

        public ILive<float> MaxHealth => maxHealth;

        public CharacterStatus(float maxHealth)
        {
            health = new Live<float>(maxHealth);
            this.maxHealth = new Live<float>(maxHealth);
        }

        public void SetHealth(float newHealth)
        {
            health.Value = Mathf.Max(newHealth, 0);
        }
    }
}