using Gast.Core.Events;
using Gast.Domain.Characters;

namespace Gast.Domain.Events
{
    public readonly struct CharacterDefeatedEvent : IDomainEvent
    {
        public ICharacter DefeatedCharacter { get; }
        public CharacterId? AttackerId { get; }

        public CharacterDefeatedEvent(ICharacter defeatedCharacter, CharacterId? attackerId)
        {
            DefeatedCharacter = defeatedCharacter;
            AttackerId = attackerId;
        }
    }
}