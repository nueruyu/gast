using Cryst.Features.CharacterAI.Humanoid.Combat;
using Cryst.Features.CharacterAI.Humanoid.Gathering;
using Cryst.Features.CharacterAI.Humanoid.Strategic;
using Gast.Lib.AI.Debugging;

namespace Cryst.Features.CharacterAI.Humanoid
{
    public class HumanoidAIBrain : AIBrainBase
    {
        readonly CombatDomainConstruct combat;
        readonly StrategicDomainConstruct strategic;
        readonly GatheringDomainConstruct gathering;
        HumanoidMemory memory;

        public HumanoidAIBrain(
            IContextRegistry contextRegistry,
            AIBrainServices services,
            CombatDomainConstruct combat,
            StrategicDomainConstruct strategic,
            GatheringDomainConstruct gathering) :
            base(contextRegistry, services)
        {
            this.combat = combat;
            this.strategic = strategic;
            this.gathering = gathering;
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
            combat.ApplyTo(registrar);
            strategic.ApplyTo(registrar);
            gathering.ApplyTo(registrar);
        }

        protected override void RegisterModules<TWorldState>(ActorContext<TWorldState> context)
        {
            context.RegisterModule(memory);
        }
    }
}
