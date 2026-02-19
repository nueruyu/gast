using System;
using System.Collections.Concurrent;

namespace Gast.Shared.Phantoms
{
    /// <summary>
    /// A pooled container that holds the actual value of a property.
    /// This is an internal detail of the Phantom implementation.
    /// </summary>
    sealed class PhantomProperty<T> : IPhantomProperty
    {
        static readonly ConcurrentStack<PhantomProperty<T>> pool = new();

        public static PhantomProperty<T> Create(T value)
        {
            if (!pool.TryPop(out var prop))
            {
                prop = new PhantomProperty<T>();
            }
            prop.Value = value;
            return prop;
        }

        public T Value { get; set; }

        PhantomProperty()
        {
        }

        public void Dispose()
        {
            Value = default;
            pool.Push(this);
        }
    }
}