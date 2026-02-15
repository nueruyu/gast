using Gast.Domain.AI;
using Cryst.Modules.CharacterAI;
using VContainer;

namespace Cryst.Infrastructure.CharacterAI
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