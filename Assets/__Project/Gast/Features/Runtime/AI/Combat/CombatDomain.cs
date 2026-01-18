using Gast.Features.AI.Combat.Actions;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;

namespace Gast.Features.AI.Combat
{
    public class CombatDomain
    {
        readonly AIDomain<CombatWorldState, AIContext<CombatWorldState>> domain;

        public CombatDomain(
            ChaseTargetAction chaseTargetAction,
            MeleeAttackAction meleeAttackAction,
            BackOffAction backOffAction,
            StrafeAction strafeAction)
        {
            domain = new AIDomainBuilder<CombatWorldState, AIContext<CombatWorldState>>()
                .RegisterAction(chaseTargetAction)
                .RegisterAction(meleeAttackAction)
                .RegisterAction(backOffAction)
                .RegisterAction(strafeAction)
                .DefineCompound("EngageTarget")
                    .AddMethod("Attack")
                        .Condition(s => s.IsInAttackRange && s.IsReadyToAttack)
                        .Do(meleeAttackAction)
                    .End()
                    .AddMethod("Withdraw")
                        .Condition(s => s.IsInAttackRange && !s.IsReadyToAttack)
                        .Do(backOffAction)
                    .End()
                    .AddMethod("Approach_Tactical")
                        .Condition(s => !s.IsInAttackRange && s.IsInCombatRange)
                        .Do(strafeAction)
                    .End()
                    .AddMethod("Chase")
                        .Condition(s => !s.IsInCombatRange)
                        .Do(chaseTargetAction)
                    .End()
                .End()
                .DefineCompound("Root")
                    .AddMethod("Combat")
                        .Condition(s => s.HasTarget)
                        .Do("EngageTarget")
                    .End()
                .End()
                .Build("Root");
        }

        public AIRunner<CombatWorldState, AIContext<CombatWorldState>> CreateRunner()
        {
            return domain.CreateRunner();
        }
    }
}