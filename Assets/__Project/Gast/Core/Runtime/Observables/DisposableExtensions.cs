using System;

namespace DescrioGames.Core.Observables
{
    /// <summary>
    /// Extension methods for IDisposable to enable fluent API.
    /// </summary>
    public static class DisposableExtensions
    {
        /// <summary>
        /// Add this disposable to a DisposableBag and return it for method chaining.
        /// </summary>
        public static T AddTo<T>(this T disposable, DisposableBag bag) where T : IDisposable
        {
            if (disposable == null)
                throw new ArgumentNullException(nameof(disposable));
            if (bag == null)
                throw new ArgumentNullException(nameof(bag));

            bag.Add(disposable);
            return disposable;
        }
    }
}
