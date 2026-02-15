using Gast.Core.Events;
using Gast.Domain.Characters;

namespace Cryst.Domain.Characters
{
    public readonly struct CharacterDefeatedEvent : IDomainEvent
    {
        public ICrystCharacter DefeatedCharacter { get; }
        public CharacterId? AttackerId { get; }

        public CharacterDefeatedEvent(ICrystCharacter defeatedCharacter, CharacterId? attackerId)
        {
            DefeatedCharacter = defeatedCharacter;
            AttackerId = attackerId;
        }
    }
}