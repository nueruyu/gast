using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using System.Threading;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Gathering.Actions
{
    public class MoveToInteractableAction : IAction<ActorContext<GatheringState>, GatheringState>
    {
        public bool CanExecute(GatheringState worldState)
        {
            return worldState.HasInteractableTarget;
        }

        public void Simulate(GatheringState worldState)
        {
            worldState.IsInRangeToInteract = true;
        }

        public async UniTask ExecuteAsync(ActorContext<GatheringState> context, CancellationToken cancellationToken)
        {
            var actor = context.Actor;
            var navigator = actor.NavigationProvider;

            try
            {
                while (!cancellationToken.IsCancellationRequested && context.WorldState.HasInteractableTarget)
                {
                    var targetPosition = context.WorldState.InteractableTargetPosition;
                    navigator.SetDestination(targetPosition);

                    if (navigator.HasArrived || context.WorldState.IsInRangeToInteract)
                    {
                        break;
                    }

                    var direction = navigator.NextSteeringDirection;
                    if (direction != Vector3.zero)
                    {
                        actor.Move(direction);
                    }

                    await UniTask.Yield(cancellationToken);
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
