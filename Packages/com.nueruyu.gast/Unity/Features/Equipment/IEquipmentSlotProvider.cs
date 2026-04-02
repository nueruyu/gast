using System.Collections.Generic;
using Gast.Unity.Infrastructure.Equipment;

namespace Gast.Unity.Features.Equipment
{
    public interface IEquipmentSlotProvider
    {
        IReadOnlyList<EquipmentSlotDefinition> Slots { get; }
    }
}
