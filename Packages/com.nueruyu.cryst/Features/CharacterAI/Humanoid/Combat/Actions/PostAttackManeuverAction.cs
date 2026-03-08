using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using Random = UnityEngine.Random;

namespace Cryst.Features.CharacterAI.Humanoid.Combat.Actions
{
    [Serializable]
    public class PostAttackManeuverAction : IAction<ActorContext<CombatState>, CombatState>
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

        public async UniTask ExecuteAsync(ActorContext<CombatState> context, CancellationToken cancellationToken)
        {
            var rand = Random.value;

            if (context.WorldState.CanGuard && rand < 0.3f)
            {
                await guardAction.ExecuteAsync(context, cancellationToken);
            }
            else if (rand < 0.7f)
            {
                await strafeAction.ExecuteAsync(context, cancellationToken);
            }
            else
            {
                await backOffAction.ExecuteAsync(context, cancellationToken);
            }
        }
    }
}
