using System;

namespace Gast.Shared.Phantoms
{
    /// <summary>
    /// A non-generic interface for phantom properties, allowing them to be stored in a common collection.
    /// This is an internal detail of the Phantom implementation.
    /// </summary>
    interface IPhantomProperty : IDisposable
    {
    }
}