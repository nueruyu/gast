using System;
using DescrioGames.Core.Commands;
using DescrioGames.Domain.Characters;
using DescrioGames.Domain.Economy;

namespace DescrioGames.Commands
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

    [Serializable]
    public readonly struct PickUpItemCommand : ICommand<bool>
    {
        public CharacterId PickerId { get; }
        public ItemId ItemId { get; }
        public int Quantity { get; }

        public PickUpItemCommand(CharacterId pickerId, ItemId itemId, int quantity)
        {
            PickerId = pickerId;
            ItemId = itemId;
            Quantity = quantity;
        }
    }
}