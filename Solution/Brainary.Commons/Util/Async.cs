namespace Brainary.Commons.Util
{
    /// <summary>
    /// Helper class to run async methods within a sync process.
    /// </summary>
    public static class Async
    {
        private static readonly TaskFactory taskFactory = 
            new(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        /// <summary>
        /// Executes an async Task method which has a void return value synchronously
        /// </summary>
        /// <param name="task">Task method to execute</param>
        public static void RunSync(Func<Task> task) => taskFactory.StartNew(task).Unwrap().GetAwaiter().GetResult();

        /// <summary>
        /// Executes an async Task<T> method which has a T return type synchronously
        /// </summary>
        /// <typeparam name="TResult">Return Type</typeparam>
        /// <param name="task">Task<T> method to execute</param>
        public static TResult RunSync<TResult>(Func<Task<TResult>> task) => taskFactory.StartNew(task).Unwrap().GetAwaiter().GetResult();
    }
}
