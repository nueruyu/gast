using System;

namespace Gast.Domain.Characters
{
    public static class CharacterTypeDefinitionExtensions
    {
        public static T GetSettings<T>(this ICharacterTypeDefinition definition)
        {
            if (definition.TryGetSettings<T>(out var value))
            {
                return value;
            }

            throw new InvalidOperationException(
                $"Settings of type '{typeof(T).Name}' not found in character type '{definition.DisplayName}'.");
        }
    }
}
