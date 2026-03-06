using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Gast.Lib.AI
{
    public class AIRunner<TActorContext, TWorldState>
       where TWorldState : class, IWorldState<TWorldState>, new()
       where TActorContext : class, IActorContext<TWorldState>
    {
        readonly ITask<TActorContext, TWorldState> rootTask;

        public AIRunner(ITask<TActorContext, TWorldState> rootTask)
        {
            this.rootTask = rootTask ?? throw new ArgumentNullException(nameof(rootTask));
        }

        public UniTask RunAsync(AIContext<TActorContext> context, CancellationToken cancellationToken)
        {
            return rootTask.RunAsync(context, cancellationToken);
        }
    }
}
