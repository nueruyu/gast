using System;
using System.Collections.Generic;
using System.Linq;
using Gast.Domain.Characters;

namespace Gast.Infrastructure.Characters
{
    public class CharacterFactoryRegistry : ICharacterFactoryRegistry
    {
        readonly Dictionary<Type, ICharacterFactory> factories;

        public CharacterFactoryRegistry(IEnumerable<ICharacterFactory> factories)
        {
            this.factories = factories.ToDictionary(f => f.ParametersType);
        }

        public ICharacterFactory Get(Type parametersType)
        {
            if (this.factories.TryGetValue(parametersType, out var factory))
            {
                return factory;
            }

            throw new KeyNotFoundException($"No ICharacterFactory registered for parameter type '{parametersType.Name}'");
        }
    }
}
