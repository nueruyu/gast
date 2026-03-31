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
        public IAction<TActorContext, TWorldState> Action { get; }

        public PrimitiveTask(IAction<TActorContext, TWorldState> action)
        {
            Action = action;
        }

        public string Name => Action.ToString();

        public UniTask<bool> ValidateAsync(
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken)
        {
            if (ReferenceEquals(this, context.CurrentlyExecutingTask))
            {
                Action.Simulate(context.WorldState);
                return UniTask.FromResult(true);
            }

            if (!Action.IsAvailable(context.WorldState))
                return UniTask.FromResult(false);

            Action.Simulate(context.WorldState);

            return UniTask.FromResult(true);
        }

        public UniTask SimulateAsync(SimulationContext<TWorldState> context, CancellationToken cancellationToken)
        {
            if (Action.IsAvailable(context.WorldState))
            {
                Action.Simulate(context.WorldState);
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
                await Action.ExecuteAsync(context.ActorContext, cancellationToken);
            }
            finally
            {
                DebugLogger.ExitTask(context.Key);
            }
        }
    }
}