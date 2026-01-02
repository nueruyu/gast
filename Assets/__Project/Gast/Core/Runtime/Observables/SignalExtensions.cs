using System;

namespace Gast.Core.Observables
{
    public static class SignalExtensions
    {
        public static ISignal<T> Select<T>(this ISignal source, Func<T> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return new SelectedVoidSignal<T>(source, selector);
        }

        public static ISignal<TR> Select<T, TR>(this ISignal<T> source, Func<T, TR> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return new SelectedSignal<T, TR>(source, selector);
        }

        public static ISignal<U> Cast<T, U>(this ISignal<T> source)
        {
            return source.Select(x => (U)(object)x);
        }

        private class SelectedSignal<T, TR> : ISignal<TR>
        {
            readonly ISignal<T> source;
            readonly Func<T, TR> selector;

            public SelectedSignal(ISignal<T> source, Func<T, TR> selector)
            {
                this.source = source;
                this.selector = selector;
            }

            public IDisposable Subscribe(Action<TR> action)
            {
                return source.Subscribe(x => action(selector(x)));
            }
        }

        private class SelectedVoidSignal<T> : ISignal<T>
        {
            readonly ISignal source;
            readonly Func<T> selector;

            public SelectedVoidSignal(ISignal source, Func<T> selector)
            {
                this.source = source;
                this.selector = selector;
            }

            public IDisposable Subscribe(Action<T> action)
            {
                return source.Subscribe(() => action(selector()));
            }
        }
    }
}