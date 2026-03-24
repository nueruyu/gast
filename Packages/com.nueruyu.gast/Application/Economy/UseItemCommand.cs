using System;
using Gast.Core.Commands;
using Gast.Domain.Characters;

namespace Gast.Application.Economy
{
    [Serializable]
    public readonly struct UseItemCommand : ICommand<bool>
    {
        public CharacterId UserId { get; }
        public int InventorySlotIndex { get; }

        public UseItemCommand(CharacterId userId, int inventorySlotIndex)
        {
            UserId = userId;
            InventorySlotIndex = inventorySlotIndex;
        }
    }
}
