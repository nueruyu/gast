using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace Gast.Features.Npcs.Actions
{
    public class MoveToInteractableAction : PrimitiveTask<StrategicWorldState>
    {
        public MoveToInteractableAction() : base("MoveToInteractableAction")
        {
        }

        protected override bool CanExecute(StrategicWorldState state)
        {
            return state.HasInteractableTarget;
        }

        protected override void Simulate(ref StrategicWorldState state)
        {
            state.IsInRangeToInteract = true;
        }

        protected override async UniTask ExecuteAsync(Context<StrategicWorldState> ctx)
        {
            var navigator = ctx.Character.NavigationProvider;

            try
            {
                while (!ctx.CancellationToken.IsCancellationRequested && ctx.CurrentState.HasInteractableTarget)
                {
                    var targetPosition = ctx.CurrentState.InteractableTargetPosition;
                    navigator.SetDestination(targetPosition);

                    if (navigator.HasArrived || ctx.CurrentState.IsInRangeToInteract)
                    {
                        break;
                    }

                    var direction = navigator.NextSteeringDirection;
                    if (direction != UnityEngine.Vector3.zero)
                    {
                        ctx.Character.Move(direction);
                    }

                    await UniTask.Yield(ctx.CancellationToken);
                }
            }
            finally
            {
                navigator.Stop();
                ctx.Character.Move(UnityEngine.Vector3.zero);
            }
        }
    }
}