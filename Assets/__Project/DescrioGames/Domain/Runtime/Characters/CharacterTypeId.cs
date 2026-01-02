using System;

namespace DescrioGames.Domain.Characters
{
    /// <summary>
    /// Identifier for a character type definition (e.g., "soldier", "villager").
    /// </summary>
    public struct CharacterTypeId : IEquatable<CharacterTypeId>
    {
        readonly string value;

        CharacterTypeId(string value)
        {
            this.value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Create a CharacterTypeId from a string.
        /// </summary>
        public static CharacterTypeId FromString(string value) => new CharacterTypeId(value);

        public override string ToString() => value;

        public bool Equals(CharacterTypeId other) => value == other.value;

        public override bool Equals(object obj) => obj is CharacterTypeId other && Equals(other);

        public override int GetHashCode() => value?.GetHashCode() ?? 0;

        public static bool operator ==(CharacterTypeId left, CharacterTypeId right) => left.Equals(right);

        public static bool operator !=(CharacterTypeId left, CharacterTypeId right) => !left.Equals(right);
    }
}
