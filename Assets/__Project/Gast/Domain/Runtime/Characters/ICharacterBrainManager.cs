using Gast.Domain.AI;

namespace Gast.Domain.Characters
{
    public interface ICharacterBrainManager
    {
        void AttachBrain(CharacterId characterId, ICharacterBrain brain);
        void DetachBrain(CharacterId characterId);
    }
}
