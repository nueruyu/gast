using System;

namespace DescrioGames.Domain.Economy
{
    /// <summary>
    /// Unique identifier for items.
    /// </summary>
    public readonly struct ItemId : IEquatable<ItemId>
    {
        readonly Guid value;

        ItemId(Guid value)
        {
            this.value = value;
        }

        public static ItemId New() => new(Guid.NewGuid());

        public static ItemId FromGuid(Guid guid) => new(guid);

        public Guid ToGuid() => value;

        public bool Equals(ItemId other) => value.Equals(other.value);

        public override bool Equals(object obj) => obj is ItemId other && Equals(other);

        public override int GetHashCode() => value.GetHashCode();

        public override string ToString() => value.ToString();

        public static bool operator ==(ItemId left, ItemId right) => left.Equals(right);

        public static bool operator !=(ItemId left, ItemId right) => !left.Equals(right);
    }
}