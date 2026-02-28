using System;
using System.Collections.Generic;

namespace Gast.Core.Observables
{
    /// <summary>
    /// Signal (event) implementation with typed payload.
    /// Thread-safety: Designed for main thread use. Uses snapshot iteration to avoid collection modification issues.
    /// </summary>
    public class Signal<T> : ISignal<T>
    {
        readonly List<Action<T>> listeners = new List<Action<T>>();

        /// <summary>
        /// Subscribe to this signal.
        /// </summary>
        public IDisposable Subscribe(Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            listeners.Add(action);
            return new Subscription(this, action);
        }

        /// <summary>
        /// Publish this signal to all subscribers.
        /// Uses snapshot iteration to safely handle unsubscribe during notification.
        /// </summary>
        public void Publish(T value)
        {
            // Create snapshot to avoid modification during iteration
            var snapshot = listeners.ToArray();

            foreach (var listener in snapshot)
            {
                listener?.Invoke(value);
            }
        }

        void Unsubscribe(Action<T> action)
        {
            listeners.Remove(action);
        }

        class Subscription : IDisposable
        {
            Signal<T> signal;
            Action<T> action;

            public Subscription(Signal<T> signal, Action<T> action)
            {
                this.signal = signal;
                this.action = action;
            }

            public void Dispose()
            {
                if (signal != null && action != null)
                {
                    signal.Unsubscribe(action);
                    signal = null;
                    action = null;
                }
            }
        }
    }

    /// <summary>
    /// Signal (event) implementation without payload.
    /// Thread-safety: Designed for main thread use. Uses snapshot iteration to avoid collection modification issues.
    /// </summary>
    public class Signal : ISignal
    {
        readonly List<Action> listeners = new List<Action>();

        /// <summary>
        /// Subscribe to this signal.
        /// </summary>
        public IDisposable Subscribe(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            listeners.Add(action);
            return new Subscription(this, action);
        }

        /// <summary>
        /// Publish this signal to all subscribers.
        /// Uses snapshot iteration to safely handle unsubscribe during notification.
        /// </summary>
        public void Publish()
        {
            // Create snapshot to avoid modification during iteration
            var snapshot = listeners.ToArray();

            foreach (var listener in snapshot)
            {
                listener?.Invoke();
            }
        }

        void Unsubscribe(Action action)
        {
            listeners.Remove(action);
        }

        class Subscription : IDisposable
        {
            Signal signal;
            Action action;

            public Subscription(Signal signal, Action action)
            {
                this.signal = signal;
                this.action = action;
            }

            public void Dispose()
            {
                if (signal != null && action != null)
                {
                    signal.Unsubscribe(action);
                    signal = null;
                    action = null;
                }
            }
        }
    }
}
