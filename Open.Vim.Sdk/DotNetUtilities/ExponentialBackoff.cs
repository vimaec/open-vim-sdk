using System;
using System.Threading.Tasks;

namespace Vim.DotNetUtilities
{
    /// <summary>
    /// Runs a work function and retries it if it fails due to a transient exception.
    /// Rethrows the last exception caught after a maximum number of retries.
    /// </summary>
    public class ExponentialBackoff<T>
    {
        private const int DefaultRetries = 10;
        private const int DefaultRetryInterval = 50; // milliseconds.

        private readonly Func<T> _workFunc;
        private readonly Func<Exception, bool> _exceptionIsTransientFunc;
        private readonly int _maxRetries;
        private readonly int _retryInterval;
        private readonly Random _rng;

        public ExponentialBackoff(
            Func<T> workFunc,
            Func<Exception, bool> exceptionIsTransientFunc,
            int maxRetries = DefaultRetries,
            int retryInterval = DefaultRetryInterval)
        {
            _workFunc = workFunc;
            _exceptionIsTransientFunc = exceptionIsTransientFunc;
            _maxRetries = maxRetries;
            _retryInterval = retryInterval;

            _rng = new Random((int) DateTime.Now.Ticks);
        }

        private TimeSpan GetDelay(int retryAttempt)
        {
            // https://en.wikipedia.org/wiki/Exponential_backoff#Example_exponential_backoff_algorithm
            var max = (int) Math.Pow(2, retryAttempt) - 1;
            var multiple = _rng.Next(0, max);
            return TimeSpan.FromMilliseconds(_retryInterval * multiple);
        }

        public async Task<T> Run()
        {
            // Inspired from: https://docs.microsoft.com/en-us/azure/architecture/patterns/retry#example
            var currentRetry = 0;

            for (;;)
            {
                try
                {
                    return _workFunc();
                }
                catch (Exception e)
                {
                    currentRetry++;

                    if (currentRetry > _maxRetries || !_exceptionIsTransientFunc(e))
                    {
                        throw;
                    }
                }

                var delay = GetDelay(currentRetry);

                await Task.Delay(delay);
            }
        }

        public static async Task<T> Run(
            Func<T> workFunc,
            Func<Exception, bool> exceptionIsTransientFunc,
            int maxRetries = DefaultRetries,
            int retryInterval = DefaultRetryInterval)
            => await new ExponentialBackoff<T>(
                workFunc,
                exceptionIsTransientFunc,
                maxRetries,
                retryInterval).Run();
    }
}
