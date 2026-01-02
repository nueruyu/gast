using System.Collections.Generic;

namespace DescrioGames.Lib.AI.Builders
{
    public class DomainBuilder<TWorldState> where TWorldState : struct
    {
        readonly Domain<TWorldState> domain = new();
        readonly Dictionary<string, ITask<TWorldState>> taskRegistry = new();

        public DomainBuilder<TWorldState> RegisterTask(ITask<TWorldState> task)
        {
            taskRegistry[task.Name] = task;
            domain.RegisterTask(task);
            return this;
        }

        public CompoundTaskBuilder<TWorldState> DefineCompound(string name)
        {
            return new CompoundTaskBuilder<TWorldState>(this, name);
        }

        public CompoundTaskBuilder<TWorldState> DefineRoot()
        {
            return new CompoundTaskBuilder<TWorldState>(this, "Root", isRoot: true);
        }

        internal DomainBuilder<TWorldState> CompleteCompound(
            string name,
            CompoundTask<TWorldState> task,
            bool isRoot)
        {
            RegisterTask(task);
            if (isRoot) domain.SetRootTask(task);
            return this;
        }

        internal ITask<TWorldState> GetTask(string name)
        {
            return taskRegistry.TryGetValue(name, out var task) ? task : null;
        }

        public Domain<TWorldState> Build() => domain;
    }
}