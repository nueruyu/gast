using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Gast.Lib.AI
{
    public class AIDomain<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        readonly ITask<TWorldState, TContext>[] tasks;
        readonly Dictionary<string, ITask<TWorldState, TContext>> taskMap = new();
        readonly ITask<TWorldState, TContext> rootTask;

        public IReadOnlyList<ITask<TWorldState, TContext>> Tasks => tasks;

        public AIDomain(
            IEnumerable<ITask<TWorldState, TContext>> tasks,
            string rootTaskName)
        {
            this.tasks = tasks.ToArray();

            foreach (var task in this.tasks)
            {
                taskMap[task.Name] = task;
            }

            rootTask = taskMap[rootTaskName];
        }

        public ITask<TWorldState, TContext> GetTask(string name)
        {
            return taskMap[name];
        }

        public AIRunner<TWorldState, TContext> CreateRunner()
        {
            return new AIRunner<TWorldState, TContext>(rootTask);
        }
    }
}