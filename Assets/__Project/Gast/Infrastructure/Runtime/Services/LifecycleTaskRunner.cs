using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VContainer.Unity;
using Cysharp.Threading.Tasks;
using DescrioGames.Core.Tasks;

namespace DescrioGames.Infrastructure.Services
{
    /// <summary>
    /// Manages and runs all registered ILifecycleTask instances.
    /// Starts all tasks when the application starts and cancels them on disposal.
    /// </summary>
    public class LifecycleTaskRunner : IStartable, IDisposable
    {
        readonly IEnumerable<ILifecycleTask> tasks;
        CancellationTokenSource cts;

        public LifecycleTaskRunner(IEnumerable<ILifecycleTask> tasks)
        {
            this.tasks = tasks;
        }

        public void Start()
        {
            cts = new CancellationTokenSource();

            foreach (var task in tasks)
            {
                RunTaskSafely(task, cts.Token).Forget();
            }
        }

        async UniTaskVoid RunTaskSafely(ILifecycleTask task, CancellationToken cancellationToken)
        {
            try
            {
                await task.RunAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public void Dispose()
        {
            cts?.Cancel();
            cts?.Dispose();
        }
    }
}