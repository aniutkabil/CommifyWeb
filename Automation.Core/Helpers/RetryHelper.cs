namespace Automation.Core.Helpers
{
    public static class RetryHelper
    {
        public static void RetryOnException(int timeoutInSeconds, Action action, int pollingMs = 250)
        {
            var end = DateTime.UtcNow.AddSeconds(timeoutInSeconds);
            Exception? last = null;

            while (DateTime.UtcNow < end)
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception ex)
                {
                    last = ex;
                    Thread.Sleep(pollingMs);
                }
            }

            throw new TimeoutException($"Action failed after {timeoutInSeconds}s. Last error: {last?.Message}", last);
        }
    }
}
