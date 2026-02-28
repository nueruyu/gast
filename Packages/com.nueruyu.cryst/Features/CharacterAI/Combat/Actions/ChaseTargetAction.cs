using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using Cryst.Domain.Characters.Facets;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI.Combat.Actions
{
    [Serializable]
    public class ChaseTargetAction : IAction<CombatState, AIContext<CombatState>>
    {
        public bool CanExecute(CombatState worldState)
        {
            return worldState.HasTarget && !worldState.IsInAttackRange;
        }

        public void Simulate(CombatState worldState)
        {
            worldState.DistanceToTarget = worldState.AttackRange;
        }

        public async UniTask ExecuteAsync(AIContext<CombatState> ctx)
        {
            var actor = ctx.Actor;
            if (actor.Character.Is(out SprintableCharacter sprintable))
            {
                sprintable.SetSprint(true);
            }

            var navigator = actor.NavigationProvider;

            try
            {
                while (!ctx.CancellationToken.IsCancellationRequested)
                {
                    var worldState = ctx.WorldState;
                    navigator.SetDestination(worldState.TargetPosition);

                    var selfToTarget = worldState.TargetPosition - actor.Body.Position;
                    selfToTarget.y = 0;

                    var currentDist = selfToTarget.magnitude;
                    if (currentDist <= worldState.AttackRange)
                    {
                        navigator.Stop();
                        if (actor.Character.Is(out SprintableCharacter sprintableOnExit))
                        {
                            sprintableOnExit.SetSprint(false);
                        }
                        return;
                    }

                    var direction = navigator.NextSteeringDirection;
                    if (direction != Vector3.zero)
                    {
                        actor.Move(direction);
                    }

                    await UniTask.Yield(ctx.CancellationToken);
                }
            }
            finally
            {
                if (actor.Character.Is(out SprintableCharacter sprintableOnFinally))
                {
                    sprintableOnFinally.SetSprint(false);
                }
                navigator.Stop();
            }
        }
    }
}
