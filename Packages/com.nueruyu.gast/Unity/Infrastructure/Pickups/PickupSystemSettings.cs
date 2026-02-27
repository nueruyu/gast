using UnityEngine;

namespace Gast.Infrastructure.Pickups
{
    [CreateAssetMenu(fileName = "PickupSystemSettings", menuName = "Gast/Pickup/System Settings")]
    public class PickupSystemSettings : ScriptableObject
    {
        [Tooltip("Fallback prefab used for dropped items if no specific prefab is defined")]
        [SerializeField]
        GameObject pickupPrefab;

        public GameObject PickupPrefab => pickupPrefab;
    }
}