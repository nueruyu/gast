using UnityEngine;

namespace DescrioGames.Infrastructure.Settings
{
    [CreateAssetMenu(fileName = "PickupSystemSettings", menuName = "DescrioGames/Pickup/System Settings")]
    public class PickupSystemSettings : ScriptableObject
    {
        [Tooltip("Fallback prefab used for dropped items if no specific prefab is defined")]
        [SerializeField]
        GameObject pickupPrefab;

        public GameObject PickupPrefab => pickupPrefab;
    }
}