using DescrioGames.Features.Characters;
using R3;
using System;

namespace DescrioGames.Features.Combat.Methods
{
    /// <summary>
    /// Empty method that does nothing.
    /// Used for characters that cannot attack.
    /// </summary>
    public class EmptyMethod : ICombatMethod
    {
        public IDisposable BindEvents(CharacterContext character)
        {
            return Disposable.Empty;
        }

        public void Attack(CharacterContext attacker)
        {
            // Do nothing
        }
    }
}