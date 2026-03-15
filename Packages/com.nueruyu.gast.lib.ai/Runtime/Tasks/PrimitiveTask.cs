using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI.Debugging;
using Gast.Lib.AI.Testing;

namespace Gast.Lib.AI.Tasks
{
    public class PrimitiveTask<TActorContext, TWorldState> : ITask<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        readonly IAction<TActorContext, TWorldState> action;

        public PrimitiveTask(IAction<TActorContext, TWorldState> action)
        {
            this.action = action;
        }

        public string Name => action.ToString();

        public UniTask<bool> ValidateAsync(
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken)
        {
            if (ReferenceEquals(this, context.CurrentlyExecutingTask))
            {
                action.Simulate(context.WorldState);
                return UniTask.FromResult(true);
            }

            if (!action.IsAvailable(context.WorldState))
                return UniTask.FromResult(false);

            action.Simulate(context.WorldState);

            return UniTask.FromResult(true);
        }

        public UniTask SimulateAsync(SimulationContext<TWorldState> context, CancellationToken cancellationToken)
        {
            if (action.IsAvailable(context.WorldState))
            {
                action.Simulate(context.WorldState);
                context.SimulatedPlan.Add(this);
            }

            return UniTask.CompletedTask;
        }

        public async UniTask RunAsync(ExecutionContext<TActorContext> context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DebugLogger.EnterTask(context.Key, Name);
            try
            {
                await action.ExecuteAsync(context.ActorContext, cancellationToken);
            }
            finally
            {
                DebugLogger.ExitTask(context.Key);
            }
        }
    }
}