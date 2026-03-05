using Gast.Lib.AI.Tasks;
using System.Collections.Generic;

namespace Gast.Lib.AI.Builders
{
    public class CompoundTaskBuilder<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        readonly List<Method<TWorldState, TContext>> methods = new();
        IMethodSelector<TWorldState, TContext> selector;

        internal int CurrentMethodCount => methods.Count;

        internal AIDomainBuilder<TWorldState, TContext> DomainBuilder { get; }
        public string Name { get; }

        internal CompoundTaskBuilder(AIDomainBuilder<TWorldState, TContext> domainBuilder, string name)
        {
            DomainBuilder = domainBuilder;
            Name = name;
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

        internal CompoundTask<TWorldState, TContext> Build()
        {
            return new CompoundTask<TWorldState, TContext>(
                Name,
                methods,
                selector ?? new MethodSelectors.PrioritySelector<TWorldState, TContext>());
        }
    }
}
