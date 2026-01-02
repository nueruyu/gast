using System;
using DescrioGames.Core.Observables;
using R3;

namespace DescrioGames.Shared.Observables
{
    /// <summary>
    /// Extension methods to bridge Core's ILive&lt;T&gt; and ISignal to R3's Observable.
    /// </summary>
    public static class LiveR3Extensions
    {
        /// <summary>
        /// Converts an ILive&lt;T&gt; to an R3 Observable&lt;T&gt;.
        /// Emits the current value immediately and then all subsequent changes.
        /// </summary>
        /// <typeparam name="T">The type of value being observed.</typeparam>
        /// <param name="live">The ILive source.</param>
        /// <returns>An Observable that emits the current value and subsequent changes.</returns>
        public static Observable<T> ToObservable<T>(this ILive<T> live)
        {
            return Observable.Create<T>(observer =>
            {
                // Subscribe with current value to emit immediately
                var subscription = live.SubscribeWithCurrent(value =>
                {
                    observer.OnNext(value);
                });

                // Return disposable that cleans up the subscription
                return subscription;
            });
        }

        /// <summary>
        /// Converts an ISignal to an R3 Observable&lt;Unit&gt;.
        /// Emits Unit.Default whenever the signal is raised.
        /// </summary>
        /// <param name="signal">The ISignal source.</param>
        /// <returns>An Observable that emits Unit.Default on each signal.</returns>
        public static Observable<Unit> ToObservable(this ISignal signal)
        {
            return Observable.Create<Unit>(observer =>
            {
                return signal.Subscribe(() => observer.OnNext(Unit.Default));
            });
        }

        /// <summary>
        /// Converts an ISignal&lt;T&gt; to an R3 Observable&lt;T&gt;.
        /// Emits the signal's value whenever the signal is raised.
        /// </summary>
        /// <typeparam name="T">The type of value being signaled.</typeparam>
        /// <param name="signal">The ISignal source.</param>
        /// <returns>An Observable that emits values on each signal.</returns>
        public static Observable<T> ToObservable<T>(this ISignal<T> signal)
        {
            return Observable.Create<T>(observer =>
            {
                return signal.Subscribe(value => observer.OnNext(value));
            });
        }
    }
}