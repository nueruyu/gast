using System.Collections.Generic;
using Gast.Lib.AI.Debugging;

namespace Cryst.Features.CharacterAI.Humanoid
{
    public class HumanoidAIBrain : AIBrainBase
    {
        readonly IEnumerable<IAIDomainDefinition> domainDefinitions;
        HumanoidMemory memory;

        public HumanoidAIBrain(
            IContextRegistry contextRegistry,
            AIBrainServices services,
            IEnumerable<IAIDomainDefinition> domainDefinitions) :
            base(contextRegistry, services)
        {
            this.domainDefinitions = domainDefinitions;
        }

        protected override void OnBrainInitialize()
        {
            memory = new HumanoidMemory();
        }

        protected override void OnBrainCleanup()
        {
            memory = null;
        }

        protected override void RegisterDomains(IDomainRegistrar registrar)
        {
            foreach (var definition in domainDefinitions)
            {
                definition.RegisterTo(registrar);
            }
        }

        protected override void RegisterModules<TWorldState>(ActorContext<TWorldState> context)
        {
            context.RegisterModule(memory);
        }
    }
}
