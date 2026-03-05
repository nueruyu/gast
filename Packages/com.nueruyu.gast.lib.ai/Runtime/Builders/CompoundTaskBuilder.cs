using Gast.Lib.AI.Tasks;
using System.Collections.Generic;

namespace Gast.Lib.AI.Builders
{
    public class CompoundTaskBuilder<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        readonly List<MethodBuilder<TWorldState, TContext>> methodBuilders = new();
        IMethodSelector<TWorldState, TContext> selector;

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
            var builder = new MethodBuilder<TWorldState, TContext>(methodName);
            methodBuilders.Add(builder);
            return builder;
        }

        internal CompoundTask<TWorldState, TContext> Build()
        {
            var builtMethods = new List<Method<TWorldState, TContext>>();
            for (var i = 0; i < methodBuilders.Count; i++)
            {
                builtMethods.Add(methodBuilders[i].Build(i));
            }

            return new CompoundTask<TWorldState, TContext>(
                Name,
                builtMethods,
                selector ?? new MethodSelectors.PrioritySelector<TWorldState, TContext>());
        }
    }
}
