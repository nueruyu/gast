using UnityEngine;

namespace Cryst.Infrastructure.Characters
{
    [CreateAssetMenu(fileName = "CharacterStatSchema", menuName = "Cryst/Character Status Settings")]
    public class CharacterStatusSettings : ScriptableObject
    {
        [SerializeField]
        float initialMaxHealth = 100f;

        [SerializeField]
        float initialMaxHunger = 100f;

        public float InitialMaxHealth => initialMaxHealth;
        public float InitialMaxHunger => initialMaxHunger;
    }
}