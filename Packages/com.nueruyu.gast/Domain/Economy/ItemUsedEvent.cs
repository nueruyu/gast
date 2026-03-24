using Gast.Core.Events;
using Gast.Domain.Characters;

namespace Gast.Domain.Economy
{
    public readonly struct ItemUsedEvent : IDomainEvent
    {
        public CharacterId UserId { get; }
        public ItemId ItemId { get; }

        public ItemUsedEvent(CharacterId userId, ItemId itemId)
        {
            UserId = userId;
            ItemId = itemId;
        }
    }
}
