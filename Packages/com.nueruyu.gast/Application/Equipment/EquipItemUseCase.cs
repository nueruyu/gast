using System.Linq;
using Gast.Core.Commands;
using Gast.Domain.Characters;
using Gast.Domain.Equipment;

namespace Gast.Application.Equipment
{
    public class EquipItemUseCase : ICommandHandler<EquipItemCommand>
    {
        readonly ICharacterRepository characterRepository;

        public EquipItemUseCase(ICharacterRepository characterRepository)
        {
            this.characterRepository = characterRepository;
        }

        public void Execute(in EquipItemCommand command)
        {
            var character = characterRepository.Get(command.CharacterId);
            if (!character.Is(out IEquipmentHost host)) return;

            foreach (var slotId in host.Slots)
            {
                if (!host.GetSlot(slotId).Value.HasValue)
                {
                    host.Equip(slotId, command.ItemId);
                    return;
                }
            }

            host.Equip(host.Slots.First(), command.ItemId);
        }
    }
}
