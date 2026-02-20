using Gast.Domain.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Gast.Features.Characters
{
    public record CharacterContext(
        CharacterId CharacaterId,
        CharacterTypeId TypeId,
        ICharacterTypeDefinition TypeDefinition,
        CharacterBody Body,
        CancellationToken CancellationToken)
    {
        readonly Dictionary<Type, object> services = new();

        public IEnumerable<(Type, object)> GetModules() => services.Select(x => (x.Key, x.Value));

        public void Register<T>(T service) where T : class
        {
            services[typeof(T)] = service;
        }

        public void Register(Type type, object service)
        {
            services[type] = service;
        }

        public T Resolve<T>() where T : class
        {
            return services.TryGetValue(typeof(T), out var service) ? (T)service : null;
        }
    }
}