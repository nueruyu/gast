using UnityEngine;

namespace Gast.Unity.Features.Placement
{
    [CreateAssetMenu(fileName = "PlacementSettings", menuName = "Gast/Placement/Placement Settings")]
    public class PlacementSettings : ScriptableObject
    {
        [Header("Preview")]
        [SerializeField]
        Material validPlacementMaterial;

        [SerializeField]
        Material invalidPlacementMaterial;

        [Header("Raycasting")]
        [SerializeField]
        float maxPlacementDistance = 10f;

        [SerializeField]
        LayerMask placementLayerMask;

        public Material ValidPlacementMaterial => validPlacementMaterial;
        public Material InvalidPlacementMaterial => invalidPlacementMaterial;
        public float MaxPlacementDistance => maxPlacementDistance;
        public LayerMask PlacementLayerMask => placementLayerMask;
    }
}
