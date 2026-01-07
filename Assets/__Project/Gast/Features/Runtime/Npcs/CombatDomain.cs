using Gast.Lib.AI;
using Gast.Lib.AI.Builders;

namespace Gast.Features.Npcs
{
    public static class CombatDomain
    {
        public static Domain<CombatWorldState> Create(SoldierBrainSettings settings)
        {
            return new DomainBuilder<CombatWorldState>()
                .RegisterTask(settings.ChaseTargetAction)
                .RegisterTask(settings.MeleeAttackAction)
                .RegisterTask(settings.BackOffAction)
                .RegisterTask(settings.StrafeAction)
                .RegisterTask(settings.IdleAction)
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
                .DefineRoot()
                    .AddMethod("Combat")
                        .Condition(s => s.HasTarget)
                        .Do("EngageTarget")
                    .End()
                    .AddMethod("Idle")
                        .Condition(s => !s.HasTarget)
                        .Do(settings.IdleAction)
                    .End()
                .End()
                .Build();
        }
    }
}
