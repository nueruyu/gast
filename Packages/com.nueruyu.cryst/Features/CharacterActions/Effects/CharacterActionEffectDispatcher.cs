using System;
using System.Collections.Generic;
using Gast.Unity.Features.Characters;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Effects
{
    /// <summary>
    /// Dispatches effects to their corresponding registered handlers.
    /// Handler lookup is based on <see cref="ICharacterActionEffectHandler.EffectType"/>.
    /// </summary>
    public class CharacterActionEffectDispatcher
    {
        readonly Dictionary<Type, ICharacterActionEffectHandler> handlers;

        public CharacterActionEffectDispatcher(IEnumerable<ICharacterActionEffectHandler> handlers)
        {
            this.handlers = new Dictionary<Type, ICharacterActionEffectHandler>();
            foreach (var handler in handlers)
            {
                this.handlers[handler.EffectType] = handler;
            }
        }

        /// <summary>
        /// Dispatches the effect to its registered handler.
        /// Logs a warning if no handler is found for the effect type.
        /// </summary>
        public void Dispatch(CharacterActionEffect effect, CharacterContext context)
        {
            if (handlers.TryGetValue(effect.GetType(), out var handler))
            {
                handler.Handle(effect, context);
            }
            else
            {
                Debug.LogWarning($"No handler found for effect type: {effect.GetType().Name}");
            }
        }
    }
}
