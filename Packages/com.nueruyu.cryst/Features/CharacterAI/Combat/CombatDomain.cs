using Cryst.Features.CharacterAI.Combat.Actions;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;
using Gast.Lib.AI.MethodSelectors;

namespace Cryst.Features.CharacterAI.Combat
{
    public class CombatDomain
    {
        readonly AIDomain<CombatState, AIContext<CombatState>> domain;

        public CombatDomain()
        {
            var builder = new AIDomainBuilder<CombatState, AIContext<CombatState>>();

            var chaseTarget = builder.RegisterAction("ChaseTarget", new ChaseTargetAction());
            var meleeAttack = builder.RegisterAction("MeleeAttack", new MeleeAttackAction());
            var strafe = builder.RegisterAction("Strafe", new StrafeAction());
            var stalk = builder.RegisterAction("Stalk", new StalkAction());
            var postAttackManeuver = builder.RegisterAction("PostAttackManeuver", new PostAttackManeuverAction());
            var idle = builder.RegisterAction("Idle", new IdleAction());

            var engageTarget = builder.DefineCompound("EngageTarget", c =>
            {
                c.UseSelector(new UtilitySelector<CombatState, AIContext<CombatState>>());
                c.AddMethod("Attack")
                    .Condition(s => s.IsInAttackRange && s.IsReadyToAttack)
                    .Score(s => 0.5f + 0.5f * s.SelfHealthRatio)
                    .InterruptCost(s => 1.0f)
                    .Do(stalk)
                    .Do(meleeAttack)
                    .End();
                c.AddMethod("Maneuver")
                    .Condition(s => s.IsInAttackRange)
                    .Score(s => 0.2f + 0.8f * (1.0f - s.SelfHealthRatio))
                    .InterruptCost(s => 0.3f)
                    .Do(postAttackManeuver)
                    .End();
                c.AddMethod("Approach_Tactical")
                    .Condition(s => !s.IsInAttackRange && s.IsInCombatRange)
                    .Score(s => 0.6f)
                    .InterruptCost(s => 0.1f)
                    .Do(strafe)
                    .End();
                c.AddMethod("Chase")
                    .Condition(s => !s.IsInCombatRange)
                    .Score(s => 0.4f)
                    .InterruptCost(s => 0.1f)
                    .Do(chaseTarget)
                    .End();
            });

            builder.DefineCompound("Root", c =>
            {
                c.AddMethod("Combat")
                    .Condition(s => s.HasTarget)
                    .Do(engageTarget)
                    .End();
                c.AddMethod("Idle")
                    .Do(idle)
                    .End();
            });

            domain = builder.Build("Root");
        }

        public AIRunner<CombatState, AIContext<CombatState>> CreateRunner()
        {
            return domain.CreateRunner();
        }
    }
}
