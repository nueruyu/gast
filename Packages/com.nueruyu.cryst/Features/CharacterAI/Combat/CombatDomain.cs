using Cryst.Features.CharacterAI.Combat.Actions;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;
using Gast.Lib.AI.MethodSelectors;

namespace Cryst.Features.CharacterAI.Combat
{
    public class CombatDomain
    {
        readonly AIDomain<ActorContext<CombatState>, CombatState> domain;

        public CombatDomain()
        {
            var builder = new AIDomainBuilder<ActorContext<CombatState>, CombatState>();

            var engageTarget = builder.DefineCompound("EngageTarget")
                .UseSelector(new UtilitySelector<ActorContext<CombatState>, CombatState>());

            engageTarget.AddMethod("Attack")
                .Condition(s => s.IsInAttackRange && s.IsReadyToAttack)
                .Score(s => 0.5f + 0.5f * s.SelfHealthRatio)
                .InterruptCost(s => 1.0f)
                .Do(new StalkAction())
                .Do(new MeleeAttackAction());
            engageTarget.AddMethod("Maneuver")
                .Condition(s => s.IsInAttackRange)
                .Score(s => 0.2f + 0.8f * (1.0f - s.SelfHealthRatio))
                .InterruptCost(s => 0.3f)
                .Do(new PostAttackManeuverAction());
            engageTarget.AddMethod("Approach_Tactical")
                .Condition(s => !s.IsInAttackRange && s.IsInCombatRange)
                .Score(s => 0.6f)
                .InterruptCost(s => 0.1f)
                .Do(new StrafeAction());
            engageTarget.AddMethod("Chase")
                .Condition(s => !s.IsInCombatRange)
                .Score(s => 0.4f)
                .InterruptCost(s => 0.1f)
                .Do(new ChaseTargetAction());

            var root = builder.DefineCompound("Root");

            root.AddMethod("Combat")
                .Condition(s => s.HasTarget)
                .Do(engageTarget);
            root.AddMethod("Idle")
                .Do(new IdleAction());

            domain = builder.Build("Root");
        }

        public AIRunner<ActorContext<CombatState>, CombatState> CreateRunner()
        {
            return domain.CreateRunner();
        }
    }
}
