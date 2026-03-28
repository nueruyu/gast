using Gast.Domain.Characters;

namespace Cryst.Application.Characters
{
    public interface ICharacterDeathService
    {
        void Kill(ICharacter character, CharacterId? attackerId);
    }
}
