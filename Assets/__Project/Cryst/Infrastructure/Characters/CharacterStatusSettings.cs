using Gast.Domain.Stats;
using Gast.Infrastructure.Stats;
using Cryst.Domain.Characters;
using UnityEngine;

namespace Cryst.Infrastructure.Characters
{
    [CreateAssetMenu(fileName = "CharacterStatSchema", menuName = "Cryst/Character Status Settings")]
    public class CharacterStatusSettings : StatSchema, IBasicStatSchema
    {
        [Header("Core Stats")]
        [SerializeField]
        FloatStatDefinition health;

        [SerializeField]
        float initialHealth = 100f;

        [SerializeField]
        FloatStatDefinition maxHealth;

        [SerializeField]
        float initialMaxHealth = 100f;

        public IStatDefinition<float> Health => health;
        public IStatDefinition<float> MaxHealth => maxHealth;

        public float InitialMaxHealth => initialMaxHealth;

        public override void Initialize(IStatRegistrar registrar)
        {
            registrar.Register(health, initialHealth);
            registrar.Register(maxHealth, initialMaxHealth);
        }
    }
}