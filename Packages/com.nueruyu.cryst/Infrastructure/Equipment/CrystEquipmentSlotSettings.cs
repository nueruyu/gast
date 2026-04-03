using System.Collections.Generic;
using Gast.Unity.Features.Equipment;
using Gast.Unity.Infrastructure.Equipment;
using UnityEngine;

namespace Cryst.Infrastructure.Equipment
{
    [CreateAssetMenu(fileName = "CrystEquipmentSlotSettings", menuName = "Cryst/Equipment/Slot Settings")]
    public class CrystEquipmentSlotSettings : ScriptableObject, ICrystEquipmentSlots, IEquipmentSlotProvider
    {
        [SerializeField]
        EquipmentSlotDefinition head;

        [SerializeField]
        EquipmentSlotDefinition body;

        [SerializeField]
        EquipmentSlotDefinition weapon;

        [System.NonSerialized]
        IReadOnlyList<EquipmentSlotDefinition> slotsCache;

        public EquipmentSlotDefinition Head => head;
        public EquipmentSlotDefinition Body => body;
        public EquipmentSlotDefinition Weapon => weapon;

        public IReadOnlyList<EquipmentSlotDefinition> Slots
        {
            get
            {
                slotsCache ??= new[] { head, body, weapon };
                return slotsCache;
            }
        }

        void OnValidate()
        {
            slotsCache = null;
        }
    }
}
