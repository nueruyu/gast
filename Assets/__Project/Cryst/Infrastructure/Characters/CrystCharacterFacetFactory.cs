using System;
using Cryst.Domain.Characters;
using Gast.Core.Events;
using Gast.Domain.Characters;

namespace Cryst.Infrastructure.Characters
{
    public class CrystCharacterFacetFactory : ICharacterFacetFactory
    {
        readonly IDomainEventPublisher eventPublisher;
        readonly ICharacterBrainManager brainManager;

        public CrystCharacterFacetFactory(
            IDomainEventPublisher eventPublisher,
            ICharacterBrainManager brainManager)
        {
            this.eventPublisher = eventPublisher;
            this.brainManager = brainManager;
        }

        public Type FacetType => typeof(ICrystCharacter);

        public ICharacterFacet Create(ICharacter character)
        {
            return new CrystCharacter(character, eventPublisher, brainManager);
        }
    }
}
