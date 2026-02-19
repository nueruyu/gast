using System;
using System.Collections.Generic;
using Gast.Domain.Characters;

namespace Gast.Infrastructure.Characters
{
    public class CharacterFacetFactoryRegistry : ICharacterFacetFactoryRegistry
    {
        readonly Dictionary<Type, ICharacterFacetFactory> factories = new();

        public CharacterFacetFactoryRegistry(IEnumerable<ICharacterFacetFactory> factories)
        {
            foreach (var factor in factories)
                Register(factor);
        }

        void Register(ICharacterFacetFactory factory)
        {
            factories[factory.FacetType] = factory;
        }

        public ICharacterFacetFactory Get(Type facetType)
        {
            factories.TryGetValue(facetType, out var factory);
            return factory;
        }
    }
}