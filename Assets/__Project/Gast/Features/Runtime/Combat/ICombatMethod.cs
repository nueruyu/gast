using Gast.Features.Characters;
using System;

namespace Gast.Features.Combat
{
    /// <summary>
    /// Strategy interface for attack implementations.
    /// </summary>
    public interface ICombatMethod
    {
        IDisposable BindEvents(CharacterContext character);

        void Attack(CharacterContext attacker);
    }
}