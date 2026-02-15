using Gast.Features.Combat;
using UnityEngine;

namespace Gast.Infrastructure.Combat
{
    [CreateAssetMenu(fileName = "DamageAreaSettings", menuName = "Gast/Combat/Damage Area Settings")]
    public class DamageAreaSettings : ScriptableObject
    {
        [SerializeField]
        DamageArea damageAreaPrefab;

        public DamageArea DamageAreaPrefab => damageAreaPrefab;
    }
}
