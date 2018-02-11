// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

namespace System.Threading.Tasks
{
    /// <summary>
    /// Helpers for safely using Task libraries.
    /// </summary>
    internal static class TaskHelpers
    {
        private static readonly Task DefaultCompleted = FromResult<AsyncVoid>(default(AsyncVoid));

        /// <summary>
        /// Returns a completed task that has no result.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        internal static Task Completed()
        {
            return DefaultCompleted;
        }

        /// <summary>
        /// Returns an error task. The task is Completed, IsCanceled = False, IsFaulted = True
        /// </summary>
        /// <param name="exception">The exception to bind to this <see cref="Task"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        internal static Task FromError(Exception exception)
        {
            return FromError<AsyncVoid>(exception);
        }

        /// <summary>
        /// Returns an error task of the given type. The task is Completed, IsCanceled = False, IsFaulted = True
        /// </summary>
        /// <param name="exception">The exception to bind to this <see cref="Task{TResult}"/>.</param>
        /// <typeparam name="TResult">The type of the result produced by this <see cref="Task{TResult}"/>.</typeparam>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        internal static Task<TResult> FromError<TResult>(Exception exception)
        {
            var taskCompletionSource = new TaskCompletionSource<TResult>();
            taskCompletionSource.SetException(exception);
            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Returns a successful completed task with the given result.
        /// </summary>
        /// <param name="result">The result value to bind to this <see cref="Task{TResult}"/>.</param>
        /// <typeparam name="TResult">The type of the result produced by this <see cref="Task{TResult}"/>.</typeparam>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        internal static Task<TResult> FromResult<TResult>(TResult result)
        {
            var taskCompletionSource = new TaskCompletionSource<TResult>();
            taskCompletionSource.SetResult(result);
            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Used as the T in a "conversion" of a Task into a Task{T}
        /// </summary>
        private struct AsyncVoid
        {
        }
    }
}
