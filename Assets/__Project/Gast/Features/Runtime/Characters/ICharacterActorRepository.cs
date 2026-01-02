using Gast.Domain.Characters;

namespace Gast.Features.Characters
{
    public interface ICharacterActorRepository
    {
        Character Get(CharacterId id);

        void Register(Character character);

        void Unregister(CharacterId id);
    }
}