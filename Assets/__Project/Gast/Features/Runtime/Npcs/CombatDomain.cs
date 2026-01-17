using Gast.Lib.AI;
using Gast.Lib.AI.Builders;

namespace Gast.Features.Npcs
{
    public static class CombatDomain
    {
        public static AIDomain<CombatWorldState, AIContext<CombatWorldState>> Create(SoldierBrainSettings settings)
        {
            return new AIDomainBuilder<CombatWorldState, AIContext<CombatWorldState>>()
                .RegisterTask(settings.ChaseTargetAction)
                .RegisterTask(settings.MeleeAttackAction)
                .RegisterTask(settings.BackOffAction)
                .RegisterTask(settings.StrafeAction)
                .DefineCompound("EngageTarget")
                    .AddMethod("Attack")
                        .Condition(s => s.IsInAttackRange && s.IsReadyToAttack)
                        .Do(settings.MeleeAttackAction)
                    .End()
                    .AddMethod("Withdraw")
                        .Condition(s => s.IsInAttackRange && !s.IsReadyToAttack)
                        .Do(settings.BackOffAction)
                    .End()
                    .AddMethod("Approach_Tactical")
                        .Condition(s => !s.IsInAttackRange && s.IsInCombatRange)
                        .Do(settings.StrafeAction)
                    .End()
                    .AddMethod("Chase")
                        .Condition(s => !s.IsInCombatRange)
                        .Do(settings.ChaseTargetAction)
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
    }
}