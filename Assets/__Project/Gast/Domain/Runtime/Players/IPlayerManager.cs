using Gast.Core.Observables;
using Gast.Domain.Characters;

namespace Gast.Domain.Players
{
    public interface IPlayerManager
    {
        ILive<ICharacter> CurrentCharacter { get; }

        void Possess(CharacterId characterId);

        void Unpossess();
    }
}