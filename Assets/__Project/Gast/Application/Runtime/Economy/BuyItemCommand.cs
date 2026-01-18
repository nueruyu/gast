using System;
using Gast.Core.Commands;
using Gast.Domain.Characters;
using Gast.Domain.Economy;

namespace Gast.Api.Economy
{
    [Serializable]
    public readonly struct BuyItemCommand : ICommand<bool>
    {
        public CharacterId BuyerId { get; }
        public ItemId ItemId { get; }

        public BuyItemCommand(CharacterId buyerId, ItemId itemId)
        {
            BuyerId = buyerId;
            ItemId = itemId;
        }
    }
}