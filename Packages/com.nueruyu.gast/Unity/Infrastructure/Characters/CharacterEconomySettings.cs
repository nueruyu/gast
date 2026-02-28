using UnityEngine;

namespace Gast.Unity.Infrastructure.Characters
{
    [CreateAssetMenu(fileName = "CharacterEconomySettings", menuName = "Gast/Characters/Settings/Economy Settings")]
    public class CharacterEconomySettings : ScriptableObject
    {
        [Header("Economy")]
        [SerializeField]
        int initialMoney = 100;

        [SerializeField]
        int slotCapacity = 20;

        public int InitialMoney => initialMoney;
        public int SlotCapacity => slotCapacity;
    }
}
