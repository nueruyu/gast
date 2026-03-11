using System;
using System.Collections.Generic;
using System.Linq;

namespace Gast.Lib.AI
{
    public class AIDomain<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        readonly ITask<TActorContext, TWorldState>[] tasks;
        readonly Dictionary<string, ITask<TActorContext, TWorldState>> taskMap = new();
        readonly ITask<TActorContext, TWorldState> rootTask;

        public IReadOnlyList<ITask<TActorContext, TWorldState>> Tasks => tasks;
        public ITask<TActorContext, TWorldState> RootTask => rootTask;

        public AIDomain(
            IEnumerable<ITask<TActorContext, TWorldState>> tasks,
            string rootTaskName)
        {
            this.tasks = tasks.ToArray();

            foreach (var task in this.tasks)
            {
                taskMap[task.Name] = task;
            }

            rootTask = taskMap[rootTaskName];
        }

        public ITask<TActorContext, TWorldState> GetTask(string name)
        {
            return taskMap[name];
        }

        public AIRunner<TActorContext, TWorldState> CreateRunner()
        {
            return new AIRunner<TActorContext, TWorldState>(rootTask);
        }
    }
}
