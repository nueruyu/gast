using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Unity.Features.Characters
{
    public record CharacterContext(
        CharacterId CharacaterId,
        GameObject GameObject,
        CancellationToken CancellationToken)
    {
        readonly Dictionary<Type, object> services = new();

        public IEnumerable<(Type, object)> GetModules() => services.Select(x => (x.Key, x.Value));

        public void Register<T>(T service) where T : class
        {
            if (service == null)
                throw new  ArgumentNullException(nameof(service));
            Register(typeof(T), service);
        }

        public void Register(Type type, object service)
        {
            services[type] = service ?? throw new  ArgumentNullException(nameof(service));
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