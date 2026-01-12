using UnityEngine;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System;

namespace Gast.Features.Npcs.Actions
{
    [Serializable]
    public class ChaseTargetAction : PrimitiveTask<CombatWorldState>
    {
        public ChaseTargetAction() : base("ChaseTarget")
        {
        }

        protected override bool CanExecute(CombatWorldState state)
        {
            return state.HasTarget && !state.IsInAttackRange;
        }

        protected override void Simulate(ref CombatWorldState state)
        {
            state.DistanceToTarget = state.AttackRange;
        }

        protected override async UniTask ExecuteAsync(Context<CombatWorldState> ctx)
        {
            ctx.Character.SetSprint(true);

            var navigator = ctx.Character.NavigationProvider;

            try
            {
                while (!ctx.CancellationToken.IsCancellationRequested)
                {
                    var currentState = ctx.CurrentState;
                    navigator.SetDestination(currentState.TargetPosition);

                    var selfToTarget = currentState.TargetPosition - ctx.Character.Body.Position;
                    selfToTarget.y = 0;

                    var currentDist = selfToTarget.magnitude;
                    if (currentDist <= currentState.AttackRange)
                    {
                        navigator.Stop();
                        ctx.Character.SetSprint(false);
                        return;
                    }

                    var direction = navigator.NextSteeringDirection;
                    if (direction != Vector3.zero)
                    {
                        ctx.Character.Move(direction);
                    }

                    await UniTask.Yield(ctx.CancellationToken);
                }
            }
            finally
            {
                ctx.Character.SetSprint(false);
                navigator.Stop();
            }
        }
    }
}