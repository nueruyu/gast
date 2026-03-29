using System;

namespace Gast.Domain.Economy
{
    /// <summary>
    /// Unique identifier for items.
    /// </summary>
    public readonly struct ItemId : IEquatable<ItemId>
    {
        readonly string value;

        ItemId(string value)
        {
            this.value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static ItemId New() => new(Guid.NewGuid().ToString());

        public static ItemId FromString(string id) => new(id);

        public bool Equals(ItemId other) => string.Equals(value, other.value, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is ItemId other && Equals(other);

        public override int GetHashCode() => value?.GetHashCode() ?? 0;

        public override string ToString() => value;

        public static bool operator ==(ItemId left, ItemId right) => left.Equals(right);

        public static bool operator !=(ItemId left, ItemId right) => !left.Equals(right);
    }
}