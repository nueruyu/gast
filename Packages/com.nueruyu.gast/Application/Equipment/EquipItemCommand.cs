using System;
using Gast.Core.Commands;
using Gast.Domain.Characters;
using Gast.Domain.Economy;

namespace Gast.Application.Equipment
{
    [Serializable]
    public readonly struct EquipItemCommand : ICommand
    {
        public CharacterId CharacterId { get; }
        public ItemId ItemId { get; }

        public EquipItemCommand(CharacterId characterId, ItemId itemId)
        {
            CharacterId = characterId;
            ItemId = itemId;
        }
    }
}
