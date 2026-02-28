namespace Gast.Core.Events
{
    /// <summary>
    /// Defines a contract for publishing domain events.
    /// </summary>
    public interface IDomainEventPublisher
    {
        /// <summary>
        /// Publishes a domain event to all subscribers.
        /// </summary>
        /// <typeparam name="T">The type of the event.</typeparam>
        /// <param name="domainEvent">The event to publish.</param>
        void Publish<T>(in T domainEvent) where T : struct, IDomainEvent;
    }
}