using System;

namespace Gast.Domain.Characters
{
    public interface ICharacterFacetFactoryRegistry
    {
        ICharacterFacetFactory Get(Type facetType);
    }
}