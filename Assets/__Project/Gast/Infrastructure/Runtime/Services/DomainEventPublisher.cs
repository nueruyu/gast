using System;
using System.Collections.Generic;
using Gast.Core.Events;
using Gast.Core.Observables;

namespace Gast.Infrastructure.Services
{
    public class DomainEventPublisher : IDomainEventPublisher, IDomainEventSubscriber
    {
        readonly Dictionary<Type, object> signals = new();

        Signal<T> GetSignal<T>() where T : struct, IDomainEvent
        {
            if (!signals.TryGetValue(typeof(T), out var signal))
            {
                signal = new Signal<T>();
                signals[typeof(T)] = signal;
            }
            return (Signal<T>)signal;
        }

        public void Publish<T>(in T domainEvent) where T : struct, IDomainEvent
        {
            GetSignal<T>().Publish(domainEvent);
        }

        public IDisposable Subscribe<T>(Action<T> handler) where T : struct, IDomainEvent
        {
            return GetSignal<T>().Subscribe(handler);
        }
    }
}