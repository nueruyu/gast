using Cryst.Modules.CharacterAI.Combat.Actions;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;
using Gast.Lib.AI.MethodSelectors;

namespace Cryst.Modules.CharacterAI.Combat
{
    public class CombatDomain
    {
        readonly AIDomain<CombatState, AIContext<CombatState>> domain;

        public CombatDomain(
            ChaseTargetAction chaseTargetAction,
            MeleeAttackAction meleeAttackAction,
            BackOffAction backOffAction,
            StrafeAction strafeAction,
            GuardAction guardAction,
            StalkAction stalkAction,
            PostAttackManeuverAction postAttackManeuverAction)
        {
            domain = new AIDomainBuilder<CombatState, AIContext<CombatState>>()
                .RegisterAction("ChaseTarget", chaseTargetAction)
                .RegisterAction("MeleeAttack", meleeAttackAction)
                .RegisterAction("BackOff", backOffAction)
                .RegisterAction("Strafe", strafeAction)
                .RegisterAction("Guard", guardAction)
                .RegisterAction("Stalk", stalkAction)
                .RegisterAction("PostAttackManeuver", postAttackManeuverAction)
                .RegisterAction("Idle", new IdleAction())
                .DefineCompound("EngageTarget")
                    .UseSelector(new UtilitySelector<CombatState, AIContext<CombatState>>())
                    .AddMethod("Attack")
                        .Condition(s => s.IsInAttackRange && s.IsReadyToAttack)
                        .Score(s => 0.5f + 0.5f * s.SelfHealthRatio) // Higher health -> Higher aggro (0.5 ~ 1.0)
                        .InterruptCost(s => 1.0f) // High cost to prevent cancelling an attack
                        .Do("Stalk", "MeleeAttack")
                    .End()
                    .AddMethod("Maneuver")
                        .Condition(s => s.IsInAttackRange)
                        .Score(s => 0.2f + 0.8f * (1.0f - s.SelfHealthRatio)) // Lower health -> Higher defensive score (0.2 ~ 1.0)
                        .InterruptCost(s => 0.3f) // Moderate cost: don't interrupt evasion easily
                        .Do("PostAttackManeuver")
                    .End()
                    .AddMethod("Approach_Tactical")
                        .Condition(s => !s.IsInAttackRange && s.IsInCombatRange)
                        .Score(s => 0.6f)
                        .InterruptCost(s => 0.1f) // Low cost
                        .Do("Strafe")
                    .End()
                    .AddMethod("Chase")
                        .Condition(s => !s.IsInCombatRange)
                        .Score(s => 0.4f)
                        .InterruptCost(s => 0.1f) // Low cost
                        .Do("ChaseTarget")
                    .End()
                .End()
                .DefineCompound("Root")
                    .AddMethod("Combat")
                        .Condition(s => s.HasTarget)
                        .Do("EngageTarget")
                    .End()
                    .AddMethod("Idle")
                        .Do("Idle")
                    .End()
                .End()
                .Build("Root");
        }

        public AIRunner<CombatState, AIContext<CombatState>> CreateRunner()
        {
            return domain.CreateRunner();
        }
    }
}