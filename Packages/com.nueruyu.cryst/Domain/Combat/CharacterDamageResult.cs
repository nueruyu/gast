namespace Cryst.Domain.Combat
{
    public enum CharacterDamageResult
    {
        /// <summary>
        /// Damage was not applied (e.g. the character is already dead).
        /// </summary>
        NoDamage,

        /// <summary>
        /// Damage was applied and the character is still alive.
        /// </summary>
        Alive,

        /// <summary>
        /// Damage was applied and the character was defeated.
        /// </summary>
        Defeated
    }
}
