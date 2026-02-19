using System;

namespace Gast.Shared.Phantoms
{
    /// <summary>
    /// A simple implementation of IPhantomKey that uses a string name to identify the key.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PhantomKey<T> : IPhantomKey<T>
    {
        public string Name { get; }

        public PhantomKey(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public override string ToString() => Name;
    }
}