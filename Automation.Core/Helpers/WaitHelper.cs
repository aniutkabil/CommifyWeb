using Automation.Core.Exceptions;

namespace Automation.Core.Helpers
{
    public static class WaitHelper
    {
        private static readonly TimeSpan DefaultTimeStep = TimeSpan.FromMilliseconds(100);

        public static bool WaitForCondition(Func<bool> condition, int timeoutInSeconds = 15, bool throwTimeoutException = false, string? errorMessage = null, TimeSpan? polling = null)
        {
            polling ??= DefaultTimeStep;

            var stopDate = DateTime.Now.AddSeconds(timeoutInSeconds);

            while (DateTime.Now < stopDate)
            {
                try
                {
                    if (condition.Invoke())
                        return true;

                    Thread.Sleep(polling.Value);
                }
                catch (Exception e)
                {
                    errorMessage ??= $"Condition failed with exception: {e.Message}";
                }
            }

            if (throwTimeoutException)
                throw new WaitException(errorMessage ?? "Condition was not met within the timeout period.");

            return false;
        }

        public static TResult? WaitResult<TResult>(Func<TResult> action, int timeoutInSeconds = 15, bool throwTimeout = false, string? errorMessage = null, TimeSpan? polling = null) where TResult : class
        {
            polling ??= DefaultTimeStep;

            var stopTime = DateTime.Now.Add(TimeSpan.FromSeconds(timeoutInSeconds));
            TResult? result = default;
            do
            {
                try
                {
                    result = action();
                }
                catch (Exception e)
                {
                    errorMessage ??= e.Message;
                }

                if (result != null)
                {
                    return result;
                }

                Thread.Sleep(polling.Value);
            }
            while (DateTime.Now < stopTime);

            if (result == null && throwTimeout)
            {
                throw new WaitException(errorMessage);
            }

            return result;
        }

        public static TResult? WaitResultWithCondition<TResult>(Func<TResult> action, Func<TResult, bool> condition, int timeoutInSeconds = 15, bool throwTimeout = false, string? errorMessage = null, TimeSpan? polling = null)
        {
            polling ??= DefaultTimeStep;

            var stopTime = DateTime.Now.Add(TimeSpan.FromSeconds(timeoutInSeconds));
            TResult? result = default;
            do
            {
                try
                {
                    result = action();
                }
                catch (Exception e)
                {
                    errorMessage ??= e.Message;
                }

                if (result != null && condition(result))
                {
                    return result;
                }
                else
                {
                    Thread.Sleep(polling.Value);
                    continue;
                }
            }
            while (DateTime.Now < stopTime);

            if ((result == null || !condition(result)) && throwTimeout)
            {
                throw new WaitException(errorMessage);
            }

            return result;
        }

        public static T? WaitActual<T>(Func<T> action, T expected, Func<T, T, bool> conditionAction, int timeoutInSeconds = 15, bool throwException = true, TimeSpan? polling = null)
        {
            polling ??= DefaultTimeStep;

            var stopTime = DateTime.Now.Add(TimeSpan.FromSeconds(timeoutInSeconds));

            var actual = default(T);
            string? errorMessage;
            do
            {
                errorMessage = null;

                try
                {
                    actual = action();
                    if (conditionAction(actual, expected))
                    {
                        return actual;
                    }
                }
                catch (Exception e)
                {
                    errorMessage = e.Message;
                }

                Thread.Sleep(polling.Value);
            }
            while (DateTime.Now < stopTime);

            if (errorMessage != null && throwException)
            {
                throw new Exception(errorMessage);
            }

            return actual;
        }

        public static T? WaitActualEquals<T>(Func<T> action, T expected, int timeoutInSeconds = 15, bool throwException = true, TimeSpan? polling = null)
        {
            return WaitActual(action, expected, EqualityComparer<T>.Default.Equals, timeoutInSeconds, throwException, polling);
        }
    }
}
