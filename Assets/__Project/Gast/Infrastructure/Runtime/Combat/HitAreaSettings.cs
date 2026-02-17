using Gast.Features.Combat;
using UnityEngine;

namespace Gast.Infrastructure.Combat
{
    [CreateAssetMenu(fileName = "HitAreaSettings", menuName = "Gast/Combat/Hit Area Settings")]
    public class HitAreaSettings : ScriptableObject
    {
        [SerializeField]
        HitArea hitAreaPrefab;

        public HitArea HitAreaPrefab => hitAreaPrefab;
    }
}