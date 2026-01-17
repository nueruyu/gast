using Cysharp.Threading.Tasks;
using Gast.Lib.AI.Tasks;
using UnityEngine;

namespace Gast.Features.Npcs.Actions
{
    public class MoveToInteractableAction : PrimitiveTask<StrategicWorldState, AIContext<StrategicWorldState>>
    {
        public MoveToInteractableAction() : base("MoveToInteractableAction")
        {
        }

        protected override bool CanExecute(StrategicWorldState worldState)
        {
            return worldState.HasInteractableTarget;
        }

        protected override void Simulate(StrategicWorldState worldState)
        {
            worldState.IsInRangeToInteract = true;
        }

        protected override async UniTask ExecuteAsync(AIContext<StrategicWorldState> ctx)
        {
            var actor = ctx.Actor;
            var navigator = actor.NavigationProvider;

            try
            {
                while (!ctx.CancellationToken.IsCancellationRequested && ctx.WorldState.HasInteractableTarget)
                {
                    var targetPosition = ctx.WorldState.InteractableTargetPosition;
                    navigator.SetDestination(targetPosition);

                    if (navigator.HasArrived || ctx.WorldState.IsInRangeToInteract)
                    {
                        break;
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
                navigator.Stop();
                actor.Move(Vector3.zero);
            }
        }
    }
}
