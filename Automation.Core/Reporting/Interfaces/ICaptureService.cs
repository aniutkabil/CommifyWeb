namespace Automation.Core.Reporting.Interfaces
{
    public interface ICaptureService
    {
        string CaptureScreenshot(string testName, string? methodName = "");
    }
}
