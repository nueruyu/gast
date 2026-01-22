using Gast.Features.AI.Combat.Actions;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;

namespace Gast.Features.AI.Combat
{
    public class CombatDomain
    {
        readonly AIDomain<CombatState, AIContext<CombatState>> domain;

        public CombatDomain(
            ChaseTargetAction chaseTargetAction,
            MeleeAttackAction meleeAttackAction,
            BackOffAction backOffAction,
            StrafeAction strafeAction)
        {
            domain = new AIDomainBuilder<CombatState, AIContext<CombatState>>()
                .RegisterAction("ChaseTarget", chaseTargetAction)
                .RegisterAction("MeleeAttack", meleeAttackAction)
                .RegisterAction("BackOff", backOffAction)
                .RegisterAction("Strafe", strafeAction)
                .RegisterAction("Idle", new IdleAction())
                .DefineCompound("EngageTarget")
                    .AddMethod("Attack")
                        .Condition(s => s.IsInAttackRange && s.IsReadyToAttack)
                        .Do("MeleeAttack")
                    .End()
                    .AddMethod("Withdraw")
                        .Condition(s => s.IsInAttackRange && !s.IsReadyToAttack)
                        .Do("BackOff")
                    .End()
                    .AddMethod("Approach_Tactical")
                        .Condition(s => !s.IsInAttackRange && s.IsInCombatRange)
                        .Do("Strafe")
                    .End()
                    .AddMethod("Chase")
                        .Condition(s => !s.IsInCombatRange)
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