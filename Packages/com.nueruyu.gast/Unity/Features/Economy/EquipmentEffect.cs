using Gast.Core.Stats;
using Gast.Domain.Equipment;
using UnityEngine;

namespace Gast.Unity.Features.Economy
{
    public abstract class EquipmentEffect : ScriptableObject, IEquipmentEffect
    {
        public abstract void ApplyTo(StatBuilder stats);
    }
}
