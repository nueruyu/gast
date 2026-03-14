using System;
using System.Threading;
using Cryst.Domain.Characters.Facets;
using Cryst.Features.CharacterAI.Common;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Humanoid.Combat.Actions
{
    [Serializable]
    public class ChaseTargetAction : IAction<ActorContext<CombatState>, CombatState>
    {
        public bool IsAvailable(CombatState worldState)
        {
            return worldState.HasTarget;
        }

        public void Simulate(CombatState worldState)
        {
            worldState.DistanceToTarget = worldState.AttackRange;
        }

        public async UniTask ExecuteAsync(
            ActorContext<CombatState> context,
            CancellationToken cancellationToken)
        {
            if (context.Character.Is(out SprintableCharacter sprintable))
                sprintable.SetSprint(true);

            try
            {
                await context.Actor.MoveToAsync(
                    static ctx => ctx.WorldState.TargetPosition,
                    static ctx =>
                    {
                        var toTarget = ctx.WorldState.TargetPosition - ctx.Actor.Body.Position;
                        toTarget.y = 0;
                        return toTarget.magnitude <= ctx.WorldState.AttackRange;
                    },
                    context,
                    cancellationToken);
            }
            finally
            {
                sprintable?.SetSprint(false);
            }
        }
    }
}