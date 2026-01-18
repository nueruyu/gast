using System;
using Gast.Core.Commands;
using Gast.Domain.Characters;
using Gast.Domain.Economy;

namespace Gast.Application.Economy
{
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