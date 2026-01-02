using System;

namespace DescrioGames.Core.Observables
{
    /// <summary>
    /// Read-only interface for signal (event) with typed payload.
    /// </summary>
    public interface ISignal<T>
    {
        /// <summary>
        /// Subscribe to this signal.
        /// </summary>
        /// <param name="action">Action to invoke when signal is published.</param>
        /// <returns>Disposable to unsubscribe.</returns>
        IDisposable Subscribe(Action<T> action);
    }

    /// <summary>
    /// Read-only interface for signal (event) without payload.
    /// </summary>
    public interface ISignal
    {
        /// <summary>
        /// Subscribe to this signal.
        /// </summary>
        /// <param name="action">Action to invoke when signal is published.</param>
        /// <returns>Disposable to unsubscribe.</returns>
        IDisposable Subscribe(Action action);
    }
}