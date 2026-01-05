using Gast.Core.Events;
using Gast.Domain.Characters;
using Gast.Domain.Economy;

namespace Gast.Domain.Events
{
    public readonly struct ItemAcquiredEvent : IDomainEvent
    {
        public CharacterId AcquirerId { get; }
        public ItemId AcquiredItemId { get; }
        public int AcquiredQuantity { get; }

        public ItemAcquiredEvent(CharacterId acquirerId, ItemId acquiredItemId, int acquiredQuantity)
        {
            AcquirerId = acquirerId;
            AcquiredItemId = acquiredItemId;
            AcquiredQuantity = acquiredQuantity;
        }
    }
}