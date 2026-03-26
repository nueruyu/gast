using UnityEngine;

namespace Cryst.Features.Characters.Vitals
{
    [CreateAssetMenu(fileName = "VitalsSettings", menuName = "Cryst/Vitals Settings")]
    public class VitalsSettings : ScriptableObject
    {
        [SerializeField]
        float hungerDecreaseRate = 0.5f;

        [SerializeField]
        float starvationDamage = 2.0f;

        public float HungerDecreaseRate => hungerDecreaseRate;
        public float StarvationDamage => starvationDamage;
    }
}
