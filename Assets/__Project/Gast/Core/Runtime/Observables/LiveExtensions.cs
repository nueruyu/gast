using System;

namespace DescrioGames.Core.Observables
{
    public static class LiveExtensions
    {
        public static ILive<TR> Select<T, TR>(this ILive<T> source, Func<T, TR> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return new SelectedLive<T, TR>(source, selector);
        }

        public static ILive<U> Cast<T, U>(this ILive<T> source)
        {
            return source.Select(x => (U)(object)x);
        }

        class SelectedLive<T, TR> : ILive<TR>
        {
            readonly ILive<T> source;
            readonly Func<T, TR> selector;

            public SelectedLive(ILive<T> source, Func<T, TR> selector)
            {
                this.source = source;
                this.selector = selector;
            }

            public TR Value => selector(source.Value);

            public IDisposable Subscribe(Action<TR> action)
            {
                return source.Subscribe(x => action(selector(x)));
            }

            public IDisposable SubscribeWithCurrent(Action<TR> action)
            {
                return source.SubscribeWithCurrent(x => action(selector(x)));
            }
        }
    }
}