using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Gast.Shared.Tasks
{
    public static class ValueTaskExtensions
    {
        public static Action<Exception> ExceptionHandler { get; set; } = ex => Debug.LogException(ex);

        public static void Forget(this ValueTask task)
        {
            if (task.IsCompletedSuccessfully)
            {
                return;
            }
            ForgetAwaited(task);
        }

        public static void Forget<T>(this ValueTask<T> task)
        {
            if (task.IsCompletedSuccessfully)
            {
                return;
            }
            ForgetAwaited(task);
        }

        static async void ForgetAwaited(ValueTask task)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                ExceptionHandler?.Invoke(ex);
            }
        }

        static async void ForgetAwaited<T>(ValueTask<T> task)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                ExceptionHandler?.Invoke(ex);
            }
        }
    }
}