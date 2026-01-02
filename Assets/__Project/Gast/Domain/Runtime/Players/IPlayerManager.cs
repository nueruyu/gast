using DescrioGames.Core.Observables;
using DescrioGames.Domain.Characters;

namespace DescrioGames.Domain.Players
{
    public interface IPlayerManager
    {
        ILive<ICharacter> CurrentCharacter { get; }

        void Possess(CharacterId characterId);

        void Unpossess();
    }
}