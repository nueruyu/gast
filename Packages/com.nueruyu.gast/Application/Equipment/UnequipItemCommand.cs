using System;
using Gast.Core.Commands;
using Gast.Domain.Characters;
using Gast.Domain.Equipment;

namespace Gast.Application.Equipment
{
    [Serializable]
    public readonly struct UnequipItemCommand : ICommand
    {
        public CharacterId CharacterId { get; }
        public EquipmentSlotId SlotId { get; }

        public UnequipItemCommand(CharacterId characterId, EquipmentSlotId slotId)
        {
            CharacterId = characterId;
            SlotId = slotId;
        }
    }
}
