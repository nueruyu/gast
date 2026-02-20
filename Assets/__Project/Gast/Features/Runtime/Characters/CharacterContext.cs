using Gast.Domain.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Gast.Features.Characters
{
    public record CharacterContext(
        CharacterId CharacaterId,
        CharacterTypeId TypeId,
        ICharacterTypeDefinition TypeDefinition,
        GameObject GameObject,
        CancellationToken CancellationToken)
    {
        readonly Dictionary<Type, object> services = new();

        public IEnumerable<(Type, object)> GetModules() => services.Select(x => (x.Key, x.Value));

        public void Register<T>(T service) where T : class
        {
            Register(typeof(T), service);
        }

        public void Register(Type type, object service)
        {
            services[type] = service;
        }

        public bool TryResolve<T>(out T service) where T : class
        {
            if (services.TryGetValue(typeof(T), out var serviceObj))
            {
                service = (T)serviceObj;
                return true;
            }

            service = null;
            return false;
        }

        public T Resolve<T>() where T : class
        {
            return services.TryGetValue(typeof(T), out var service) ?
                (T)service :
                throw new KeyNotFoundException($"Key '{typeof(T)}' not found");
        }
    }
}