using UnityEngine;

namespace Gast.Features.Navigations
{
    [CreateAssetMenu(fileName = "NavMeshNavigatorSettings", menuName = "Gast/Navigations/NavMesh Navigator Settings")]
    public class NavMeshNavigatorSettings : ScriptableObject
    {
        [SerializeField]
        float stoppingDistance = 0.5f;

        public float StoppingDistance => stoppingDistance;
    }
}
