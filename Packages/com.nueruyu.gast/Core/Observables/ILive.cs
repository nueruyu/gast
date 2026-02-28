using System;

namespace Gast.Core.Observables
{
    /// <summary>
    /// Read-only interface for a live property that holds a current value and notifies on changes.
    /// </summary>
    public interface ILive<T>
    {
        /// <summary>
        /// Current value of this property.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Subscribe to value changes.
        /// Will be notified only when the value changes.
        /// </summary>
        IDisposable Subscribe(Action<T> action);

        /// <summary>
        /// Subscribe to value changes with immediate notification of the current value.
        /// The action will be called immediately with the current value,
        /// then called again whenever the value changes.
        /// </summary>
        IDisposable SubscribeWithCurrent(Action<T> action);
    }
}