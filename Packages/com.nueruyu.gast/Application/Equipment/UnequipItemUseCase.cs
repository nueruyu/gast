using Gast.Core.Commands;
using Gast.Domain.Characters;
using Gast.Domain.Equipment;

namespace Gast.Application.Equipment
{
    public class UnequipItemUseCase : ICommandHandler<UnequipItemCommand>
    {
        readonly ICharacterRepository characterRepository;

        public UnequipItemUseCase(ICharacterRepository characterRepository)
        {
            this.characterRepository = characterRepository;
        }

        public void Execute(in UnequipItemCommand command)
        {
            var character = characterRepository.Get(command.CharacterId);
            if (!character.Is(out IEquipmentHost host)) return;

            host.Unequip(command.SlotId);
        }
    }
}
