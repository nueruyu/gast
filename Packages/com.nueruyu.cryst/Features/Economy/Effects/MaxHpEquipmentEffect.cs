using Cryst.Domain.Characters;
using UnityEngine;

namespace Cryst.Features.Economy.Effects
{
    [CreateAssetMenu(menuName = "Cryst/Equipment Effects/Max Hp")]
    public class MaxHpEquipmentEffect : EquipmentEffect
    {
        [SerializeField]
        float amount;

        public override void ApplyTo(StatBuilder stats)
        {
            stats.MaxHpBonus += amount;
        }
    }
}
