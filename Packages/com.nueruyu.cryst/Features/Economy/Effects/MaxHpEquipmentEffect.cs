using Cryst.Domain.Characters;
using Gast.Core.Stats;
using Gast.Unity.Features.Economy;
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
            stats.Set(CharacterStatus.MaxHpKey, stats.Get(CharacterStatus.MaxHpKey) + amount);
        }
    }
}
