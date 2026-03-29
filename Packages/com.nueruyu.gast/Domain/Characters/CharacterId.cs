using System;

namespace Gast.Domain.Characters
{
    /// <summary>
    /// Unique identifier for a character instance.
    /// </summary>
    public readonly struct CharacterId : IEquatable<CharacterId>
    {
        readonly string value;

        CharacterId(string value)
        {
            this.value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Generate a new unique CharacterId.
        /// </summary>
        public static CharacterId New() => new CharacterId(Guid.NewGuid().ToString());

        /// <summary>
        /// Create a CharacterId from a string.
        /// </summary>
        public static CharacterId FromString(string id) => new CharacterId(id);

        public bool Equals(CharacterId other) => string.Equals(value, other.value, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is CharacterId other && Equals(other);

        public override int GetHashCode() => value?.GetHashCode() ?? 0;

        public override string ToString() => value;

        public static bool operator ==(CharacterId left, CharacterId right) => left.Equals(right);

        public static bool operator !=(CharacterId left, CharacterId right) => !left.Equals(right);
    }
}
