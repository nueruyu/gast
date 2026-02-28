using UnityEngine;

namespace Gast.Unity.Features.Gameplay
{
    /// <summary>
    /// Marker component indicating where the player character should spawn.
    /// Place this on an empty GameObject to define the initial player position.
    /// </summary>
    public class PlayerSpawnPoint : MonoBehaviour
    {
#if UNITY_EDITOR

        void OnDrawGizmos()
        {
            // Draw a visual indicator in the editor
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);

            // Draw forward direction
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, transform.forward * 1.5f);
        }

        void OnDrawGizmosSelected()
        {
            // Highlight when selected
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.DrawSphere(transform.position, 0.5f);
        }

#endif
    }
}