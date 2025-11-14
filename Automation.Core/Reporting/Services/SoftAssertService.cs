using Automation.Core.Reporting.Models;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text;

namespace Automation.Core.Reporting.Services
{
    public class SoftAssertService
    {
        // Stores assertion failures
        public readonly ConcurrentBag<AssertFailInfo> Failures = new();

        // Simplified logger
        private void Log(string message) => Console.WriteLine(message);

        /// <summary>
        /// Perform an assertion and catch exceptions
        /// </summary>
        public void That(
            Action assertion,
            Func<string>? messageFactory = null,
            [CallerMemberName] string callerMethod = "",
            [CallerFilePath] string callerFile = "",
            [CallerLineNumber] int callerLine = 0)
        {
            try
            {
                assertion.Invoke();
            }
            catch (Exception ex)
            {
                var message = messageFactory?.Invoke() ?? ex.Message;
                Failures.Add(new AssertFailInfo
                {
                    Method = callerMethod,
                    SourceFilePath = callerFile,
                    SourceLineNumber = callerLine,
                    VerifyMessage = message,
                    ExceptionMessage = ex.Message
                });

                Log($"Assertion failed at {callerFile}:{callerLine} in {callerMethod}() - {message}");
            }
        }

        /// <summary>
        /// Assert multiple actions
        /// </summary>
        public void ThatAll(params Action[] assertions)
        {
            int index = 1;
            foreach (var assertion in assertions)
            {
                That(() => assertion(), () => $"Assertion #{index}");
                index++;
            }
        }

        /// <summary>
        /// Call at the end of scenario to throw if any assertion failed
        /// </summary>
        public void AssertAll()
        {
            if (!Failures.IsEmpty)
            {
                var sb = new StringBuilder();
                sb.AppendLine("SoftAssert Failures:");

                int count = 1;
                foreach (var fail in Failures)
                {
                    sb.AppendLine($"\n[{count++}] {fail.Method}() at {fail.SourceFilePath}:{fail.SourceLineNumber}");
                    if (!string.IsNullOrEmpty(fail.VerifyMessage))
                        sb.AppendLine($"Message: {fail.VerifyMessage}");
                    if (!string.IsNullOrEmpty(fail.ExceptionMessage))
                        sb.AppendLine($"Error: {fail.ExceptionMessage}");
                }

                Failures.Clear();
                throw new Exception(sb.ToString());
            }
        }
    }
}
