using Gast.Domain.Stats;
using Gast.Infrastructure.Stats;
using Cryst.Domain.Characters;
using UnityEngine;

namespace Cryst.Infrastructure.Characters
{
    [CreateAssetMenu(fileName = "CharacterStatSchema", menuName = "Cryst/Character Stat Schema")]
    public class CharacterStatSchema : StatSchema, IBasicStatSchema
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

        public override void Initialize(IStatRegistrar registrar)
        {
            registrar.Register(health, initialHealth);
            registrar.Register(maxHealth, initialMaxHealth);
        }
    }
}