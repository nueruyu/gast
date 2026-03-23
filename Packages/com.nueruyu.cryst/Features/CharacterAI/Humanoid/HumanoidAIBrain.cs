using Cryst.Features.CharacterAI.Humanoid.Combat;
using Cryst.Features.CharacterAI.Humanoid.Gathering;
using Cryst.Features.CharacterAI.Humanoid.Objective;
using Cryst.Features.CharacterAI.Humanoid.Patrol;
using Cryst.Features.CharacterAI.Humanoid.Strategic;
using Gast.Lib.AI.Debugging;

namespace Cryst.Features.CharacterAI.Humanoid
{
    public class HumanoidAIBrain : AIBrainBase
    {
        readonly CombatDomainConstruct combat;
        readonly StrategicDomainConstruct strategic;
        readonly GatheringDomainConstruct gathering;
        readonly PatrolDomainConstruct patrol;
        readonly ObjectiveDomainConstruct objective;
        HumanoidMemory memory;

        public HumanoidAIBrain(
            IContextRegistry contextRegistry,
            AIBrainServices services,
            ObjectiveManager objectiveManager,
            CombatDomainConstruct combat,
            StrategicDomainConstruct strategic,
            GatheringDomainConstruct gathering,
            PatrolDomainConstruct patrol,
            ObjectiveDomainConstruct objective) :
            base(contextRegistry, services, objectiveManager)
        {
            this.combat = combat;
            this.strategic = strategic;
            this.gathering = gathering;
            this.patrol = patrol;
            this.objective = objective;
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
            registrar.RegisterPreUpdate(() => memory.Update(Actor));

            strategic.ApplyTo(registrar);
            objective.ApplyTo(registrar, ObjectiveManager);
            combat.ApplyTo(registrar);
            gathering.ApplyTo(registrar);
            patrol.ApplyTo(registrar);
        }

        protected override void RegisterModules<TWorldState>(ActorContext<TWorldState> context)
        {
            context.RegisterModule(memory);
        }
    }
}
