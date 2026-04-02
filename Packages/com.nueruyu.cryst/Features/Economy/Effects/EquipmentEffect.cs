using Cryst.Domain.Characters;
using UnityEngine;

namespace Cryst.Features.Economy.Effects
{
    public abstract class EquipmentEffect : ScriptableObject
    {
        public abstract void ApplyTo(StatBuilder stats);
    }
}
