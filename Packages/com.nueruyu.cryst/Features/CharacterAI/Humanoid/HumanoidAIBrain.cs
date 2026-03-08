using Cryst.Features.CharacterAI.Humanoid.Combat;
using Cryst.Features.CharacterAI.Humanoid.Gathering;
using Cryst.Features.CharacterAI.Humanoid.Strategic;
using Gast.Lib.AI.Debugging;

namespace Cryst.Features.CharacterAI.Humanoid
{
    public class HumanoidAIBrain : AIBrainBase
    {
        readonly CombatDomainDefinition combatDomain;
        readonly StrategicDomainDefinition strategicDomain;
        readonly GatheringDomainDefinition gatheringDomain;
        HumanoidMemory memory;

        public HumanoidAIBrain(
            IContextRegistry contextRegistry,
            AIBrainServices services,
            CombatDomainDefinition combatDomain,
            StrategicDomainDefinition strategicDomain,
            GatheringDomainDefinition gatheringDomain) :
            base(contextRegistry, services)
        {
            this.combatDomain = combatDomain;
            this.strategicDomain = strategicDomain;
            this.gatheringDomain = gatheringDomain;
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
            combatDomain.RegisterTo(registrar);
            strategicDomain.RegisterTo(registrar);
            gatheringDomain.RegisterTo(registrar);
        }

        protected override void RegisterModules<TWorldState>(ActorContext<TWorldState> context)
        {
            context.RegisterModule(memory);
        }
    }
}
