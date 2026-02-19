using System;

namespace Gast.Domain.Characters
{
    public interface ICharacterFacetFactory
    {
        Type FacetType { get; }

        ICharacterFacet Create(ICharacter character);
    }
}