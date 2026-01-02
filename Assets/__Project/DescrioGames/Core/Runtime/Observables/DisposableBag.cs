using System;
using System.Collections.Generic;

namespace DescrioGames.Core.Observables
{
    /// <summary>
    /// Container for managing multiple IDisposable objects.
    /// Disposes all contained items when disposed.
    /// </summary>
    public class DisposableBag : IDisposable
    {
        readonly List<IDisposable> disposables = new List<IDisposable>();
        bool isDisposed;

        /// <summary>
        /// Add a disposable to this bag.
        /// </summary>
        public void Add(IDisposable item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (isDisposed)
            {
                item.Dispose();
            }
            else
            {
                disposables.Add(item);
            }
        }

        /// <summary>
        /// Clear the bag by disposing all items and emptying the list.
        /// </summary>
        public void Clear()
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }

            disposables.Clear();
        }

        /// <summary>
        /// Dispose all items in the bag and clear the list.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed)
                return;

            Clear();
            isDisposed = true;
        }
    }
}