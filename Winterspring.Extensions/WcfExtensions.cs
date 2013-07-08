using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Winterspring.Extensions
{
    public static class WcfExtensions
    {
        public static Task<TResult> ToBegin<TResult>(this Task<TResult> task, AsyncCallback callback, object state)
        {
            if (task.AsyncState == state)
            {
                if (callback != null)
                {
                    task.ContinueWith(delegate { callback(task); },
                        CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);
                }
                return task;
            }

            var tcs = new TaskCompletionSource<TResult>(state);
            task.ContinueWith(delegate
            {
                if (task.IsFaulted) tcs.TrySetException(task.Exception.InnerExceptions);
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else tcs.TrySetResult(task.Result);

                if (callback != null) callback(tcs.Task);

            }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);
            return tcs.Task;
        }

        public static TResult ToEnd<TResult>(this IAsyncResult result)
        {
            try
            {
                return ((Task<TResult>)result).Result;
            }
            catch (AggregateException ex)
            {
                // Note: the original stack trace is lost by this re-throw, but it doesn't really matter.
                throw ex.InnerException;
            }
        }
    }
}
