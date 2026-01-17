using Gast.Lib.AI.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Gast.Lib.AI.Builders
{
    public class CompoundTaskBuilder<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        readonly AIDomainBuilder<TWorldState, TContext> domainBuilder;
        readonly string taskName;
        readonly List<Method<TWorldState, TContext>> methods = new();

        int localDepthLimit = -1;
        IMethodSelector<TWorldState, TContext> selector;

        internal int CurrentMethodCount => methods.Count;

        internal CompoundTaskBuilder(AIDomainBuilder<TWorldState, TContext> domainBuilder, string taskName)
        {
            this.domainBuilder = domainBuilder;
            this.taskName = taskName;
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

        public AIDomainBuilder<TWorldState, TContext> End()
        {
            var compoundTask = new CompoundTask<TWorldState, TContext>(
                taskName,
                methods,
                selector ?? new MethodSelectors.PrioritySelector<TWorldState, TContext>(),
                localDepthLimit);

            return domainBuilder.CompleteCompound(compoundTask);
        }

        internal AIDomainBuilder<TWorldState, TContext> DomainBuilder => domainBuilder;
    }
}