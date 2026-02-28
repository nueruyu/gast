using System;

namespace Gast.Domain.Characters
{
    /// <summary>
    /// Identifier for a character archetype, which groups similar character types.
    /// </summary>
    public struct CharacterArchetypeId : IEquatable<CharacterArchetypeId>
    {
        readonly string value;

        CharacterArchetypeId(string value)
        {
            this.value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Create a CharacterArchetypeId from a string.
        /// </summary>
        public static CharacterArchetypeId FromString(string value) => new CharacterArchetypeId(value);

        public override string ToString() => value;

        public bool Equals(CharacterArchetypeId other) => value == other.value;

        public override bool Equals(object obj) => obj is CharacterArchetypeId other && Equals(other);

        public override int GetHashCode() => value?.GetHashCode() ?? 0;

        public static bool operator ==(CharacterArchetypeId left, CharacterArchetypeId right) => left.Equals(right);

        public static bool operator !=(CharacterArchetypeId left, CharacterArchetypeId right) => !left.Equals(right);
    }
}
