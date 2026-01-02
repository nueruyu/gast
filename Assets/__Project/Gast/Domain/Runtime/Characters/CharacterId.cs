using System;

namespace Gast.Domain.Characters
{
    /// <summary>
    /// Unique identifier for a character instance.
    /// </summary>
    public struct CharacterId : IEquatable<CharacterId>
    {
        readonly Guid value;

        CharacterId(Guid value)
        {
            this.value = value;
        }

        /// <summary>
        /// Generate a new unique CharacterId.
        /// </summary>
        public static CharacterId New() => new CharacterId(Guid.NewGuid());

        /// <summary>
        /// Create a CharacterId from an existing Guid.
        /// </summary>
        public static CharacterId FromGuid(Guid guid) => new CharacterId(guid);

        public Guid ToGuid() => value;

        public bool Equals(CharacterId other) => value.Equals(other.value);

        public override bool Equals(object obj) => obj is CharacterId other && Equals(other);

        public override int GetHashCode() => value.GetHashCode();

        public override string ToString() => value.ToString();

        public static bool operator ==(CharacterId left, CharacterId right) => left.Equals(right);

        public static bool operator !=(CharacterId left, CharacterId right) => !left.Equals(right);
    }
}
