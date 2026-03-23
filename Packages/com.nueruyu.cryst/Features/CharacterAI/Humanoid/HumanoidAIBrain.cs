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
        readonly AIModeMemory aiModeMemory;
        readonly CombatMemory combatMemory;
        readonly GatheringMemory gatheringMemory;

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

            aiModeMemory = new AIModeMemory();
            combatMemory = new CombatMemory();
            gatheringMemory = new GatheringMemory();
        }

        protected override void OnBrainInitialize()
        {
        }

        protected override void OnBrainCleanup()
        {
        }

        protected override void RegisterDomains(IDomainRegistrar registrar)
        {
            registrar.RegisterPreUpdate(() => combatMemory.Update(Actor));
            registrar.RegisterPreUpdate(() => gatheringMemory.Update());

            strategic.ApplyTo(registrar);
            objective.ApplyTo(registrar, ObjectiveManager);
            combat.ApplyTo(registrar);
            gathering.ApplyTo(registrar);
            patrol.ApplyTo(registrar);
        }

        protected override void RegisterModules<TWorldState>(ActorContext<TWorldState> context)
        {
            context.RegisterModule(aiModeMemory);
            context.RegisterModule(combatMemory);
            context.RegisterModule(gatheringMemory);
        }
    }
}
