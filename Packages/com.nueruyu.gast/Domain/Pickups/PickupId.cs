using System;

namespace Gast.Domain.Pickups
{
    public readonly struct PickupId : IEquatable<PickupId>
    {
        readonly string value;

        PickupId(string value) => this.value = value ?? throw new ArgumentNullException(nameof(value));

        public static PickupId New() => new PickupId(Guid.NewGuid().ToString());

        public static PickupId FromString(string id) => new PickupId(id);

        public bool Equals(PickupId other) => string.Equals(value, other.value, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is PickupId other && Equals(other);

        public override int GetHashCode() => value?.GetHashCode() ?? 0;

        public override string ToString() => value;

        public static bool operator ==(PickupId left, PickupId right) => left.Equals(right);

        public static bool operator !=(PickupId left, PickupId right) => !left.Equals(right);
    }
}