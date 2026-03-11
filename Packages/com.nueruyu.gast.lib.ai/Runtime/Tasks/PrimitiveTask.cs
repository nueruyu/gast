using Cysharp.Threading.Tasks;
using Gast.Lib.AI.Debugging;
using Gast.Lib.AI.Testing;
using System.Threading;

namespace Gast.Lib.AI.Tasks
{
    public class PrimitiveTask<TActorContext, TWorldState> : ITask<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        readonly IAction<TActorContext, TWorldState> action;

        public string Name => action.ToString();

        public PrimitiveTask(IAction<TActorContext, TWorldState> action)
        {
            this.action = action;
        }

        public UniTask<bool> ValidateAsync(
            TWorldState worldState,
            CancellationToken cancellationToken)
        {
            if (!action.CanExecute(worldState))
                return UniTask.FromResult(false);

            action.Simulate(worldState);

            return UniTask.FromResult(true);
        }

        public async UniTask SimulateAsync(SimulationContext<TWorldState> context, CancellationToken cancellationToken)
        {
            if (await ValidateAsync(context.WorldState, cancellationToken))
            {
                context.SimulatedPlan.Add(this);
            }
        }

        public async UniTask RunAsync(AIContext<TActorContext> context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var contextKey = context.Key;

            DebugLogger.EnterTask(contextKey, Name);
            try
            {
                await action.ExecuteAsync(context.ActorContext, cancellationToken);
            }
            finally
            {
                DebugLogger.ExitTask(contextKey);
            }
        }
    }
}
