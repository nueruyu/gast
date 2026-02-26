using Gast.Domain.Characters;

namespace Gast.Domain.AI
{
    public interface ICharacterAIBrainFactory
    {
        ICharacterAIBrain Create();
    }
}