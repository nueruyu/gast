using Gast.Domain.AI;
using GastGame.AI;
using VContainer;

namespace GastGame.Adapters
{
    public class CharacterAIBrainFactory : ICharacterAIBrainFactory
    {
        readonly IObjectResolver resolver;

        public CharacterAIBrainFactory(IObjectResolver resolver)
        {
            this.resolver = resolver;
        }

        public ICharacterAIBrain Create()
        {
            return resolver.Resolve<AIBrain>();
        }
    }
}