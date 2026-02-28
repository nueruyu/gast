using Gast.Unity.Features.HitDetection;
using UnityEngine;

namespace Gast.Unity.Infrastructure.HitDetection
{
    [CreateAssetMenu(fileName = "HitAreaSettings", menuName = "Gast/HitDetection/Hit Area Settings")]
    public class HitAreaSettings : ScriptableObject
    {
        [SerializeField]
        HitArea hitAreaPrefab;

        public HitArea HitAreaPrefab => hitAreaPrefab;
    }
}