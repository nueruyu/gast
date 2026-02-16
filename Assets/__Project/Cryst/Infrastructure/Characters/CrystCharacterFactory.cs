using System;
using Gast.Domain.Characters;
using Cryst.Domain.Characters;
using Gast.Core.Events;

namespace Cryst.Infrastructure.Characters
{
    public class CrystCharacterFactory : ICharacterAspectFactory
    {
        readonly IDomainEventPublisher eventPublisher;

        public CrystCharacterFactory(IDomainEventPublisher eventPublisher)
        {
            this.eventPublisher = eventPublisher;
        }

        public Type AspectType => typeof(ICrystCharacter);

        public ICharacterAspect Create(ICharacter character)
        {
            return new CrystCharacter(character, eventPublisher);
        }
    }
}