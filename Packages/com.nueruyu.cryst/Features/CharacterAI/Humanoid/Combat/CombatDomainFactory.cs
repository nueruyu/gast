using Cryst.Features.CharacterAI.Actions;
using Cryst.Features.CharacterAI.Humanoid.Combat.Actions;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;
using Gast.Lib.AI.MethodSelectors;

namespace Cryst.Features.CharacterAI.Humanoid.Combat
{
    public class CombatDomainFactory : IAIDomainFactory<CombatState>
    {
        public static class Settings
        {
            public static StalkActionSettings Stalk { get; set; } = new();
            public static MeleeAttackActionSettings MeleeAttack { get; set; } = new();
            public static PostAttackManeuverActionSettings PostAttackManeuver { get; set; } = new();
            public static StrafeActionSettings Strafe { get; set; } = new();
            public static BackOffActionSettings BackOff { get; set; } = new();
            public static GuardActionSettings Guard { get; set; } = new();
        }

        readonly AIDomain<ActorContext<CombatState>, CombatState> domain;

        public CombatDomainFactory()
        {
            var builder = new AIDomainBuilder<ActorContext<CombatState>, CombatState>();

            // Create action instances with settings
            var stalkAction = new StalkAction(Settings.Stalk);
            var meleeAttackAction = new MeleeAttackAction(Settings.MeleeAttack);

            var backOffAction = new BackOffAction(Settings.BackOff);
            var guardAction = new GuardAction(Settings.Guard);
            var strafeAction = new StrafeAction(Settings.Strafe);

            var postAttackManeuverAction = new PostAttackManeuverAction(
                guardAction,
                strafeAction,
                backOffAction,
                Settings.PostAttackManeuver);

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
