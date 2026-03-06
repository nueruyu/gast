using Gast.Lib.AI.Tasks;
using System.Collections.Generic;

namespace Gast.Lib.AI.Builders
{
    public class CompoundTaskBuilder<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TActorContext : class, IActorContext<TWorldState>
    {
        readonly List<MethodBuilder<TActorContext, TWorldState>> methodBuilders = new();
        IMethodSelector<TActorContext, TWorldState> selector;

        internal AIDomainBuilder<TActorContext, TWorldState> DomainBuilder { get; }
        public string Name { get; }

        internal CompoundTaskBuilder(AIDomainBuilder<TActorContext, TWorldState> domainBuilder, string name)
        {
            DomainBuilder = domainBuilder;
            Name = name;
        }

        public CompoundTaskBuilder<TActorContext, TWorldState> UseSelector(IMethodSelector<TActorContext, TWorldState> selector)
        {
            this.selector = selector;
            return this;
        }

        public MethodBuilder<TActorContext, TWorldState> AddMethod(string methodName)
        {
            var builder = new MethodBuilder<TActorContext, TWorldState>(methodName);
            methodBuilders.Add(builder);
            return builder;
        }

        internal CompoundTask<TActorContext, TWorldState> Build()
        {
            var builtMethods = new List<Method<TActorContext, TWorldState>>();
            for (var i = 0; i < methodBuilders.Count; i++)
            {
                builtMethods.Add(methodBuilders[i].Build(i));
            }

            return new CompoundTask<TActorContext, TWorldState>(
                Name,
                builtMethods,
                selector ?? new MethodSelectors.PrioritySelector<TActorContext, TWorldState>());
        }
    }
}
