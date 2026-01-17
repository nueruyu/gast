using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using Gast.Lib.AI;

namespace Gast.Features.Npcs.Actions
{
    [Serializable]
    public class ChaseTargetAction : IAction<CombatWorldState, AIContext<CombatWorldState>>
    {
        public string Name => "ChaseTarget";

        public bool CanExecute(CombatWorldState worldState)
        {
            return worldState.HasTarget && !worldState.IsInAttackRange;
        }

        public void Simulate(CombatWorldState worldState)
        {
            worldState.DistanceToTarget = worldState.AttackRange;
        }

        public async UniTask ExecuteAsync(AIContext<CombatWorldState> ctx)
        {
            var actor = ctx.Actor;
            actor.SetSprint(true);

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
                        actor.SetSprint(false);
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
                actor.SetSprint(false);
                navigator.Stop();
            }
        }
    }
}
