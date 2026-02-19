using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Gast.Shared.Phantoms
{
    /// <summary>
    /// A generic, pooled data container that allows storing and retrieving typed data using keys.
    /// Designed to be used with a 'using' statement to ensure it is returned to the pool.
    /// </summary>
    public sealed class Phantom : IDisposable
    {
        static readonly ConcurrentStack<Phantom> pool = new();

        /// <summary>
        /// Creates or retrieves a Phantom instance from the pool.
        /// </summary>
        public static Phantom Create()
        {
            return pool.TryPop(out var phantom) ? phantom : new Phantom();
        }

        readonly Dictionary<object, IPhantomProperty> properties = new();

        Phantom()
        {
        }

        /// <summary>
        /// Sets a value for a given key. If the key already exists, the value is updated.
        /// </summary>
        public void Set<T>(IPhantomKey<T> key, T value)
        {
            Set<IPhantomKey<T>, T>(key, value);
        }

        /// <summary>
        /// Sets a value for a given key, avoiding boxing for struct keys. If the key already exists, the value is updated.
        /// </summary>
        public void Set<TPhantomKey, T>(TPhantomKey key, T value)
            where TPhantomKey : IPhantomKey<T>
        {
            if (properties.TryGetValue(key, out var existingProp))
            {
                if (existingProp is PhantomProperty<T> typedProp)
                {
                    typedProp.Value = value;
                    return;
                }

                existingProp.Dispose();
            }

            properties[key] = PhantomProperty<T>.Create(value);
        }

        /// <summary>
        /// Tries to get a value associated with the specified key.
        /// </summary>
        public bool TryGet<T>(IPhantomKey<T> key, out T value)
        {
            return TryGet<IPhantomKey<T>, T>(key, out value);
        }

        /// <summary>
        /// Tries to get a value associated with the specified key, avoiding boxing for struct keys.
        /// </summary>
        public bool TryGet<TPhantomKey, T>(TPhantomKey key, out T value)
            where TPhantomKey : IPhantomKey<T>
        {
            if (properties.TryGetValue(key, out var propObj))
            {
                if (propObj is PhantomProperty<T> typedProp)
                {
                    value = typedProp.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the value associated with the specified key, or a default value if the key is not found.
        /// </summary>
        public T Get<T>(IPhantomKey<T> key, T defaultValue = default)
        {
            return Get<IPhantomKey<T>, T>(key, defaultValue);
        }

        /// <summary>
        /// Gets the value associated with the specified key, avoiding boxing for struct keys. Returns a default value if the key is not found.
        /// </summary>
        public T Get<TPhantomKey, T>(TPhantomKey key, T defaultValue = default)
            where TPhantomKey : IPhantomKey<T>
        {
            return TryGet(key, out var val) ? val : defaultValue;
        }

        /// <summary>
        /// Checks if a key exists in the Phantom.
        /// </summary>
        public bool Has<T>(IPhantomKey<T> key)
        {
            return Has<IPhantomKey<T>, T>(key);
        }

        /// <summary>
        /// Checks if a key exists in the Phantom, avoiding boxing for struct keys.
        /// </summary>
        public bool Has<TPhantomKey, T>(TPhantomKey key)
            where TPhantomKey : IPhantomKey<T>
        {
            return properties.ContainsKey(key);
        }

        /// <summary>
        /// Removes a key and its associated value from the Phantom.
        /// The internal property container is returned to its pool.
        /// </summary>
        public void Remove<T>(IPhantomKey<T> key)
        {
            Remove<IPhantomKey<T>, T>(key);
        }

        /// <summary>
        /// Removes a key and its associated value from the Phantom, avoiding boxing for struct keys.
        /// The internal property container is returned to its pool.
        /// </summary>
        public void Remove<TPhantomKey, T>(TPhantomKey key)
            where TPhantomKey : IPhantomKey<T>
        {
            if (properties.TryGetValue(key, out var prop))
            {
                prop.Dispose();
                properties.Remove(key);
            }
        }

        /// <summary>
        /// Returns the Phantom and all its properties to their respective pools.
        /// </summary>
        public void Dispose()
        {
            foreach (var prop in properties.Values)
            {
                prop.Dispose();
            }
            properties.Clear();

            pool.Push(this);
        }
    }
}