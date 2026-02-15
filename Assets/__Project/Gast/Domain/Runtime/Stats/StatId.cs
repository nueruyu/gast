using System;

namespace Gast.Domain.Stats
{
    /// <summary>
    /// Unique identifier for a stat definition (e.g., "Health", "Stamina").
    /// </summary>
    public readonly struct StatId : IEquatable<StatId>
    {
        readonly string value;

        StatId(string value)
        {
            this.value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static StatId FromString(string value) => new(value);

        public override string ToString() => value;

        public bool Equals(StatId other) => value == other.value;

        public override bool Equals(object obj) => obj is StatId other && Equals(other);

        public override int GetHashCode() => value?.GetHashCode() ?? 0;

        public static bool operator ==(StatId left, StatId right) => left.Equals(right);

        public static bool operator !=(StatId left, StatId right) => !left.Equals(right);
    }
}
