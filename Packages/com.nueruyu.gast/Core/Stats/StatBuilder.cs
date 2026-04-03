using System;
using System.Collections.Generic;

namespace Gast.Core.Stats
{
    public class StatBuilder
    {
        readonly Dictionary<Type, object> containers = new();

        Dictionary<object, T> GetContainer<T>()
        {
            var type = typeof(T);
            if (!containers.TryGetValue(type, out var c))
            {
                c = new Dictionary<object, T>();
                containers[type] = c;
            }
            return (Dictionary<object, T>)c;
        }

        public T Get<T>(TypedKey<T> key)
        {
            var container = GetContainer<T>();
            return container.TryGetValue(key, out var v) ? v : default;
        }

        public void Set<T>(TypedKey<T> key, T value)
        {
            GetContainer<T>()[key] = value;
        }
    }
}
