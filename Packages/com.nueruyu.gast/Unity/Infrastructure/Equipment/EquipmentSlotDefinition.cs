using Gast.Domain.Equipment;
using UnityEngine;

namespace Gast.Unity.Infrastructure.Equipment
{
    [CreateAssetMenu(fileName = "EquipmentSlotDefinition", menuName = "Gast/Equipment/Slot Definition")]
    public class EquipmentSlotDefinition : ScriptableObject
    {
        [SerializeField]
        string slotId;

        [SerializeField]
        string displayName;

        public EquipmentSlotId Id => EquipmentSlotId.FromString(slotId);
        public string DisplayName => displayName;
    }
}
