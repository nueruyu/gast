using System;

namespace Gast.Domain.Pickups
{
    public readonly struct PickupId : IEquatable<PickupId>
    {
        readonly Guid value;

        PickupId(Guid value) => this.value = value;

        public static PickupId New() => new PickupId(Guid.NewGuid());

        public static PickupId FromGuid(Guid guid) => new PickupId(guid);

        public Guid ToGuid() => value;

        public bool Equals(PickupId other) => value.Equals(other.value);

        public override bool Equals(object obj) => obj is PickupId other && Equals(other);

        public override int GetHashCode() => value.GetHashCode();

        public override string ToString() => value.ToString();

        public static bool operator ==(PickupId left, PickupId right) => left.Equals(right);

        public static bool operator !=(PickupId left, PickupId right) => !left.Equals(right);
    }
}