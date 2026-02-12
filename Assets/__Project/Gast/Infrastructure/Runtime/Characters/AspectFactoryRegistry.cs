using System;
using System.Collections.Generic;
using Gast.Domain.Characters;

namespace Gast.Infrastructure.Characters
{
    public class AspectFactoryRegistry : IAspectFactoryRegistry
    {
        readonly Dictionary<Type, IAspectFactory> factories = new();

        public AspectFactoryRegistry(IEnumerable<IAspectFactory> factories)
        {
            foreach (var factor in factories)
                Register(factor);
        }

        void Register(IAspectFactory factory)
        {
            factories[factory.AspectType] = factory;
        }

        public IAspectFactory Get(Type aspectType)
        {
            factories.TryGetValue(aspectType, out var factory);
            return factory;
        }
    }
}