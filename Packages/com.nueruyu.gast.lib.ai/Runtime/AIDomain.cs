using System.Collections.Generic;
using System.Linq;

namespace Gast.Lib.AI
{
    public class AIDomain<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        readonly Dictionary<string, ITask<TActorContext, TWorldState>> taskMap = new();
        readonly ITask<TActorContext, TWorldState>[] tasks;

        public AIDomain(
            IEnumerable<ITask<TActorContext, TWorldState>> tasks,
            string rootTaskName)
        {
            this.tasks = tasks.ToArray();

            foreach (var task in this.tasks) taskMap[task.Name] = task;

            RootTask = GetTask(rootTaskName);
        }

        public IReadOnlyList<ITask<TActorContext, TWorldState>> Tasks => tasks;
        public ITask<TActorContext, TWorldState> RootTask { get; }

        public ITask<TActorContext, TWorldState> GetTask(string name)
        {
            return taskMap[name];
        }
    }
}