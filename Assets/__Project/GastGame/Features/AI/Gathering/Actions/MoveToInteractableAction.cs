using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using UnityEngine;

namespace GastGame.AI.Gathering.Actions
{
    public class MoveToInteractableAction : IAction<GatheringState, AIContext<GatheringState>>
    {
        public bool CanExecute(GatheringState worldState)
        {
            return worldState.HasInteractableTarget;
        }

        public void Simulate(GatheringState worldState)
        {
            worldState.IsInRangeToInteract = true;
        }

        public async UniTask ExecuteAsync(AIContext<GatheringState> ctx)
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
