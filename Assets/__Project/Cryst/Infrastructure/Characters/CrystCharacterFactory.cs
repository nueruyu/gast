using System;
using Gast.Domain.Characters;
using Cryst.Domain.Characters;
using Gast.Core.Events;

namespace Cryst.Infrastructure.Characters
{
    public class CrystCharacterFactory : ICharacterFacetFactory
    {
        readonly IDomainEventPublisher eventPublisher;

        public CrystCharacterFactory(IDomainEventPublisher eventPublisher)
        {
            this.eventPublisher = eventPublisher;
        }

        public Type FacetType => typeof(ICrystCharacter);

        public ICharacterFacet Create(ICharacter character)
        {
            return new CrystCharacter(character, eventPublisher);
        }
    }
}