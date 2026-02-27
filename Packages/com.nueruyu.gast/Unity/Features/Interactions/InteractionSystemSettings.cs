using UnityEngine;

namespace Gast.Unity.Features.Interactions
{
    /// <summary>
    /// Settings for InteractionDetector.
    /// </summary>
    [CreateAssetMenu(fileName = "InteractionSystemSettings", menuName = "Gast/Interactions/System Settings")]
    public class InteractionSystemSettings : ScriptableObject
    {
        [Header("Detection")]
        [SerializeField, Tooltip("Layer mask for interactable objects")]
        LayerMask interactableLayer = -1;

        [SerializeField, Tooltip("Interval between detection checks (seconds)")]
        float detectionInterval = 0.1f;

        public LayerMask InteractableLayer => interactableLayer;
        public float DetectionInterval => detectionInterval;
    }
}