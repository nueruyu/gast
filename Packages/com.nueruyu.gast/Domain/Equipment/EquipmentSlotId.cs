using System;

namespace Gast.Domain.Equipment
{
    public readonly struct EquipmentSlotId : IEquatable<EquipmentSlotId>
    {
        public string Value { get; }

        public EquipmentSlotId(string value) => Value = value;

        public static EquipmentSlotId FromString(string value) => new(value);

        public bool Equals(EquipmentSlotId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is EquipmentSlotId other && Equals(other);
        public override int GetHashCode() => Value?.GetHashCode() ?? 0;
        public static bool operator ==(EquipmentSlotId a, EquipmentSlotId b) => a.Equals(b);
        public static bool operator !=(EquipmentSlotId a, EquipmentSlotId b) => !a.Equals(b);
        public override string ToString() => Value;
    }
}
