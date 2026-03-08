using System.Collections.Generic;
using Gast.Lib.AI.Debugging;

namespace Cryst.Features.CharacterAI
{
    public class AIBrain : BaseAIBrain
    {
        readonly IEnumerable<IAIDomainDefinition> domainDefinitions;

        public AIBrain(
            IContextRegistry contextRegistry,
            AIBrainServices services,
            IEnumerable<IAIDomainDefinition> domainDefinitions) :
            base(contextRegistry, services)
        {
            this.domainDefinitions = domainDefinitions;
        }

        protected override void RegisterDomains(IDomainRegistrar registrar)
        {
            foreach (var definition in domainDefinitions)
            {
                definition.RegisterTo(registrar);
            }
        }
    }
}
