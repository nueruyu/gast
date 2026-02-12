using System;
using System.Collections.Generic;
using Gast.Domain.Characters;

namespace Gast.Infrastructure.Characters
{
    public class CharacterAspectFactoryRegistry : ICharacterAspectFactoryRegistry
    {
        readonly Dictionary<Type, ICharacterAspectFactory> factories = new();

        public CharacterAspectFactoryRegistry(IEnumerable<ICharacterAspectFactory> factories)
        {
            foreach (var factor in factories)
                Register(factor);
        }

        void Register(ICharacterAspectFactory factory)
        {
            factories[factory.AspectType] = factory;
        }

        public ICharacterAspectFactory Get(Type aspectType)
        {
            factories.TryGetValue(aspectType, out var factory);
            return factory;
        }
    }
}