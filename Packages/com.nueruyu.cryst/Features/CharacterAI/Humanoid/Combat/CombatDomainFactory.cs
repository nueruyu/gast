using Cryst.Features.CharacterAI.Actions;
using Cryst.Features.CharacterAI.Humanoid.Combat.Actions;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;
using Gast.Lib.AI.MethodSelectors;

namespace Cryst.Features.CharacterAI.Humanoid.Combat
{
    public class CombatDomainFactory : IAIDomainFactory<CombatState>
    {
        readonly StalkActionSettings stalkSettings = new();
        readonly MeleeAttackActionSettings meleeAttackSettings = new();
        readonly PostAttackManeuverActionSettings postAttackManeuverSettings = new();
        readonly StrafeActionSettings strafeSettings = new();
        readonly BackOffActionSettings backOffSettings = new();
        readonly GuardActionSettings guardSettings = new();

        readonly AIDomain<ActorContext<CombatState>, CombatState> domain;

        public CombatDomainFactory()
        {
            var builder = new AIDomainBuilder<ActorContext<CombatState>, CombatState>();

            // Create action instances with settings
            var stalkAction = new StalkAction(stalkSettings);
            var meleeAttackAction = new MeleeAttackAction(meleeAttackSettings);

            var backOffAction = new BackOffAction(backOffSettings);
            var guardAction = new GuardAction(guardSettings);
            var strafeAction = new StrafeAction(strafeSettings);

            var postAttackManeuverAction = new PostAttackManeuverAction(
                guardAction,
                strafeAction,
                backOffAction,
                postAttackManeuverSettings);

            var chaseTargetAction = new ChaseTargetAction();
            var idleAction = new IdleAction();

            var engageTarget = builder.DefineCompound("EngageTarget")
                .UseSelector(new UtilitySelector<ActorContext<CombatState>, CombatState>());

            engageTarget.AddMethod("Attack")
                .Condition(s => s.IsInAttackRange && s.IsReadyToAttack)
                .Score(s => 0.5f + 0.5f * s.SelfHealthRatio)
                .InterruptCost(s => 1.0f)
                .Do(stalkAction)
                .Do(meleeAttackAction);
            engageTarget.AddMethod("Maneuver")
                .Condition(s => s.IsInAttackRange)
                .Score(s => 0.2f + 0.8f * (1.0f - s.SelfHealthRatio))
                .InterruptCost(s => 0.3f)
                .Do(postAttackManeuverAction);
            engageTarget.AddMethod("Approach_Tactical")
                .Condition(s => !s.IsInAttackRange && s.IsInCombatRange)
                .Score(s => 0.6f)
                .InterruptCost(s => 0.1f)
                .Do(strafeAction);
            engageTarget.AddMethod("Chase")
                .Condition(s => !s.IsInCombatRange)
                .Score(s => 0.4f)
                .InterruptCost(s => 0.1f)
                .Do(chaseTargetAction);

            var root = builder.DefineCompound("Root");

            root.AddMethod("Combat")
                .Condition(s => s.HasTarget)
                .Do(engageTarget);
            root.AddMethod("Idle")
                .Do(idleAction);

            domain = builder.Build("Root");
        }

        public AIDomain<ActorContext<CombatState>, CombatState> CreateDomain()
        {
            return domain;
        }
    }
}
