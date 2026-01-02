using System;
using System.Collections.Generic;

namespace DescrioGames.Core.Observables
{
    /// <summary>
    /// A property that holds a current value and notifies subscribers when it changes.
    /// Only notifies when the value actually changes (uses EqualityComparer).
    /// </summary>
    public class Live<T> : ILive<T>
    {
        T currentValue;
        readonly Signal<T> onChanged = new Signal<T>();

        /// <summary>
        /// Create a new LiveProperty with an initial value.
        /// </summary>
        public Live(T initialValue)
        {
            currentValue = initialValue;
        }

        /// <summary>
        /// Current value of this property.
        /// Setting a new value will notify subscribers only if the value changed.
        /// </summary>
        public T Value
        {
            get => currentValue;
            set
            {
                // Only update and notify if value actually changed
                if (!EqualityComparer<T>.Default.Equals(currentValue, value))
                {
                    currentValue = value;
                    onChanged.Publish(value);
                }
            }
        }

        /// <summary>
        /// Subscribe to value changes.
        /// Will be notified only when the value changes.
        /// </summary>
        public IDisposable Subscribe(Action<T> action)
        {
            return onChanged.Subscribe(action);
        }

        /// <summary>
        /// Subscribe to value changes with immediate notification of the current value.
        /// The action will be called immediately with the current value,
        /// then called again whenever the value changes.
        /// </summary>
        public IDisposable SubscribeWithCurrent(Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            // Immediately invoke with current value
            action(currentValue);

            // Then subscribe to future changes
            return onChanged.Subscribe(action);
        }
    }
}