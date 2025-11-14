namespace Automation.Core.Reporting.Models
{
    public class AssertFailInfo
    {
        public string? VerifyMessage { get; set; }
        public string? ExceptionMessage { get; set; }
        public string? Method { get; set; }
        public string? ScreenshotPath { get; set; }
        public string? SourceFilePath { get; set; }
        public int? SourceLineNumber { get; set; }

        public override string ToString()
        {
            return $"Method: {Method}\n Message: {VerifyMessage}\n Stacktrace: {ExceptionMessage}\n" +
                $"Screenshot: {ScreenshotPath}";
        }
    }
}
