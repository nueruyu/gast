using System;

namespace Gast.Unity.Features.Characters
{
    /// <summary>
    ///     Interface for handling a specific type of <see cref="CharacterActionEffect" />.
    /// </summary>
    public interface ICharacterActionEffectHandler
    {
        /// <summary>The effect type this handler is responsible for.</summary>
        Type EffectType { get; }

        /// <summary>Handles the given effect.</summary>
        void Handle(CharacterActionEffect effect);
    }

    /// <summary>
    ///     Type-safe generic handler interface for a specific <see cref="CharacterActionEffect" /> type.
    ///     <see cref="ICharacterActionEffectHandler.EffectType" /> and the non-generic
    ///     <see cref="ICharacterActionEffectHandler.Handle" /> are provided via default implementations.
    /// </summary>
    /// <typeparam name="T">The effect type to handle.</typeparam>
    public interface ICharacterActionEffectHandler<in T> : ICharacterActionEffectHandler where T : CharacterActionEffect
    {
        Type ICharacterActionEffectHandler.EffectType => typeof(T);

        void ICharacterActionEffectHandler.Handle(CharacterActionEffect effect)
        {
            Handle((T)effect);
        }

        /// <summary>Handles the given effect in a type-safe manner.</summary>
        void Handle(T effect);
    }
}