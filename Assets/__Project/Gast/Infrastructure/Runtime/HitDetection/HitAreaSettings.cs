using Gast.Features.HitDetection;
using UnityEngine;

namespace Gast.Infrastructure.HitDetection
{
    [CreateAssetMenu(fileName = "HitAreaSettings", menuName = "Gast/HitDetection/Hit Area Settings")]
    public class HitAreaSettings : ScriptableObject
    {
        [SerializeField]
        HitArea hitAreaPrefab;

        public HitArea HitAreaPrefab => hitAreaPrefab;
    }
}