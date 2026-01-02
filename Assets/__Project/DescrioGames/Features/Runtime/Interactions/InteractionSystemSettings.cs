using UnityEngine;

namespace DescrioGames.Features.Interactions
{
    /// <summary>
    /// Settings for InteractionDetector.
    /// </summary>
    [CreateAssetMenu(fileName = "InteractionSystemSettings", menuName = "DescrioGames/Interactions/System Settings")]
    public class InteractionSystemSettings : ScriptableObject
    {
        [Header("Detection")]
        [SerializeField, Tooltip("Radius for detecting interactable objects")]
        float detectionRadius = 2f;

        [SerializeField, Tooltip("Layer mask for interactable objects")]
        LayerMask interactableLayer = -1;

        [SerializeField, Tooltip("Interval between detection checks (seconds)")]
        float detectionInterval = 0.1f;

        public float DetectionRadius => detectionRadius;
        public LayerMask InteractableLayer => interactableLayer;
        public float DetectionInterval => detectionInterval;
    }
}