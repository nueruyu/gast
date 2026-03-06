using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;
using System.Threading;
using Random = UnityEngine.Random;
using ActorContext_ = Cryst.Features.CharacterAI.ActorContext<Cryst.Features.CharacterAI.Combat.CombatState>;

namespace Cryst.Features.CharacterAI.Combat.Actions
{
    [Serializable]
    public class PostAttackManeuverAction : IAction<ActorContext_, CombatState>
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

        public async UniTask ExecuteAsync(ActorContext_ context, CancellationToken cancellationToken)
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
