using Cysharp.Threading.Tasks;

namespace DescrioGames.Lib.AI
{
    public abstract class PrimitiveTask<TWorldState> : ITask<TWorldState>
        where TWorldState : struct
    {
        public string Name { get; }

        protected PrimitiveTask(string name) => Name = name;

        public bool Validate(
            ref TWorldState state,
            CheckOptions options,
            ISimulationContext context,
            IEnvironmentModel<TWorldState> environment = null)
        {
            if (!CheckCondition(state)) return false;

            if (options.MaxDepth != 0)
            {
                context.Clear();
                ApplyEffect(ref state, context);
                environment?.Simulate(ref state, context);
            }

            return true;
        }

        public async UniTask RunAsync(Context<TWorldState> ctx)
        {
            ctx.Token.ThrowIfCancellationRequested();
            DebugLogger.LogExecutingAction(Name);
            await ExecuteAsync(ctx);
        }

        protected abstract bool CheckCondition(TWorldState state);

        protected abstract void ApplyEffect(ref TWorldState state, ISimulationContext context);

        protected abstract UniTask ExecuteAsync(Context<TWorldState> ctx);
    }
}