using System.Collections.Generic;
using Gast.Domain.Equipment;

namespace Gast.Domain.Economy
{
    public interface IItemDefinition
    {
        public ItemId Id { get; }
        public string Name { get; }
        public int Price { get; }
        public int MaxStack { get; }
        public string Description { get; }
        IReadOnlyList<IItemEffect> Effects { get; }
        IReadOnlyList<IEquipmentEffect> EquipmentEffects { get; }
    }
}