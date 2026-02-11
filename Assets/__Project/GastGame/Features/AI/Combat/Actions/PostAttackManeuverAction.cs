using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;
using Random = UnityEngine.Random;

namespace GastGame.Features.AI.Combat.Actions
{
    /// <summary>
    /// This action decides and executes a single maneuver (Guard, Strafe, or BackOff)
    /// after an attack, and commits to it for its duration. This prevents flip-flopping
    /// between choices every frame.
    /// </summary>
    [Serializable]
    public class PostAttackManeuverAction : IAction<CombatState, AIContext<CombatState>>
    {
        readonly GuardAction guardAction;
        readonly StrafeAction strafeAction;
        readonly BackOffAction backOffAction;

        public PostAttackManeuverAction(
            GuardAction guardAction,
            StrafeAction strafeAction,
            BackOffAction backOffAction)
        {
            this.guardAction = guardAction;
            this.strafeAction = strafeAction;
            this.backOffAction = backOffAction;
        }

        public bool CanExecute(CombatState worldState)
        {
            // Can be executed as long as we are in attack range.
            // Even if IsReadyToAttack is true, the Utility Selector might choose this over Attack based on health.
            return worldState.IsInAttackRange;
        }

        public void Simulate(CombatState worldState)
        {
            // For planning purposes, we assume the most impactful state change,
            // which is increasing the distance to the target.
            backOffAction.Simulate(worldState);
        }

        public async UniTask ExecuteAsync(AIContext<CombatState> ctx)
        {
            var rand = Random.value;

            // 30% chance to guard if possible
            if (ctx.WorldState.CanGuard && rand < 0.3f)
            {
                await guardAction.ExecuteAsync(ctx);
            }
            // 40% chance to strafe (total 70%)
            else if (rand < 0.7f)
            {
                await strafeAction.ExecuteAsync(ctx);
            }
            // 30% chance to back off
            else
            {
                await backOffAction.ExecuteAsync(ctx);
            }
        }
    }
}