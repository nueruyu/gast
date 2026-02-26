using System;

namespace Gast.Core.Events
{
    /// <summary>
    /// Defines a contract for subscribing to domain events.
    /// </summary>
    public interface IDomainEventSubscriber
    {
        /// <summary>
        /// Subscribes to a specific type of domain event.
        /// </summary>
        /// <typeparam name="T">The type of the event to subscribe to.</typeparam>
        /// <param name="handler">The action to execute when the event is published.</param>
        /// <returns>An IDisposable that can be used to unsubscribe.</returns>
        IDisposable Subscribe<T>(Action<T> handler) where T : struct, IDomainEvent;
    }
}