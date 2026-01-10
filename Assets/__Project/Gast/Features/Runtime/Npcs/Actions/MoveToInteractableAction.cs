using Cysharp.Threading.Tasks;
using Gast.Lib.AI;

namespace Gast.Features.Npcs.Actions
{
    public class MoveToInteractableAction : PrimitiveTask<StrategicWorldState>
    {
        public MoveToInteractableAction() : base("MoveToInteractableAction")
        {
        }

        protected override bool CheckCondition(StrategicWorldState state)
        {
            return state.HasInteractableTarget;
        }

        protected override void ApplyEffect(ref StrategicWorldState state, ISimulationContext context)
        {
            state.IsInRangeToInteract = true;
        }

        protected override async UniTask ExecuteAsync(Context<StrategicWorldState> ctx)
        {
            var navigator = ctx.Character.NavigationProvider;

            try
            {
                while (!ctx.Token.IsCancellationRequested && ctx.CurrentState.HasInteractableTarget)
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

                    await UniTask.Yield(ctx.Token);
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