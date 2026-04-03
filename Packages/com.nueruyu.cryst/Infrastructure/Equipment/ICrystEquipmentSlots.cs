using Gast.Unity.Infrastructure.Equipment;

namespace Cryst.Infrastructure.Equipment
{
    public interface ICrystEquipmentSlots
    {
        EquipmentSlotDefinition Head { get; }
        EquipmentSlotDefinition Body { get; }
        EquipmentSlotDefinition Weapon { get; }
    }
}
