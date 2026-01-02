using System.Collections.Generic;

namespace DescrioGames.Lib.AI
{
    public class Domain<TWorldState> where TWorldState : struct
    {
        readonly Dictionary<string, ITask<TWorldState>> tasks = new();

        public ITask<TWorldState> RootTask { get; private set; }

        public void RegisterTask(ITask<TWorldState> task)
        {
            tasks[task.Name] = task;
        }

        public void SetRootTask(ITask<TWorldState> root)
        {
            RootTask = root;
        }

        public ITask<TWorldState> GetTask(string name)
        {
            return tasks.TryGetValue(name, out var task) ? task : null;
        }
    }
}