using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;
using Random = UnityEngine.Random;

namespace Cryst.Features.CharacterAI.Combat.Actions
{
    [Serializable]
    public class PostAttackManeuverAction : IAction<CombatState, AIContext<CombatState>>
    {
        readonly GuardAction guardAction = new();
        readonly StrafeAction strafeAction = new();
        readonly BackOffAction backOffAction = new();

        public PostAttackManeuverAction(GuardAction guardAction, StrafeAction strafeAction, BackOffAction backOffAction)
        {
        }

        public PostAttackManeuverAction() {}

        public bool CanExecute(CombatState worldState)
        {
            return worldState.IsInAttackRange;
        }

        public void Simulate(CombatState worldState)
        {
            backOffAction.Simulate(worldState);
        }

        public async UniTask ExecuteAsync(AIContext<CombatState> ctx)
        {
            var rand = Random.value;

            if (ctx.WorldState.CanGuard && rand < 0.3f)
            {
                await guardAction.ExecuteAsync(ctx);
            }
            else if (rand < 0.7f)
            {
                await strafeAction.ExecuteAsync(ctx);
            }
            else
            {
                await backOffAction.ExecuteAsync(ctx);
            }
        }
    }
}
