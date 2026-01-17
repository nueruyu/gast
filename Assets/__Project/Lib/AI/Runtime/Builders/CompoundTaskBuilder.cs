using System.Collections.Generic;

namespace Gast.Lib.AI.Builders
{
    public class CompoundTaskBuilder<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        readonly DomainBuilder<TWorldState, TContext> domainBuilder;
        readonly string taskName;
        readonly bool isRoot;
        readonly List<Method<TWorldState, TContext>> methods = new();

        int localDepthLimit = -1;
        IMethodSelector<TWorldState, TContext> selector;

        internal int CurrentMethodCount => methods.Count;

        internal CompoundTaskBuilder(DomainBuilder<TWorldState, TContext> domainBuilder, string taskName, bool isRoot = false)
        {
            this.domainBuilder = domainBuilder;
            this.taskName = taskName;
            this.isRoot = isRoot;
        }

        public CompoundTaskBuilder<TWorldState, TContext> CheckDepth(int depth)
        {
            localDepthLimit = depth;
            return this;
        }

        public CompoundTaskBuilder<TWorldState, TContext> UseSelector(IMethodSelector<TWorldState, TContext> selector)
        {
            this.selector = selector;
            return this;
        }

        public MethodBuilder<TWorldState, TContext> AddMethod(string methodName)
        {
            return new MethodBuilder<TWorldState, TContext>(this, methodName);
        }

        internal CompoundTaskBuilder<TWorldState, TContext> CompleteMethod(Method<TWorldState, TContext> method)
        {
            methods.Add(method);
            return this;
        }

        public DomainBuilder<TWorldState, TContext> End()
        {
            var compoundTask = new CompoundTask<TWorldState, TContext>(taskName)
            {
                LocalDepthLimit = localDepthLimit,
            };

            if (selector != null)
            {
                compoundTask.Selector = selector;
            }

            foreach (var method in methods)
            {
                compoundTask.Methods.Add(method);
            }

            return domainBuilder.CompleteCompound(taskName, compoundTask, isRoot);
        }

        internal DomainBuilder<TWorldState, TContext> DomainBuilder => domainBuilder;
    }
}