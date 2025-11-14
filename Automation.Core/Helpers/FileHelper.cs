namespace Automation.Core.Helpers
{
    public static class FileHelper
    {
        public static bool WaitUntilFileIsFound(string directory, string fileName, bool @throwException = false, int timeoutInSec = 10)
        {
            string filePath = Path.Combine(directory, fileName);

            return WaitHelper.WaitForCondition(() =>
            {
                return File.Exists(filePath) && new FileInfo(filePath).Length > 0;
            }, timeoutInSec, @throwException, $"File {fileName} has NOT been found in directory {directory} within {timeoutInSec} seconds");
        }

        public static void DeleteFile(string directory, string fileName)
        {
            string filePath = Path.Combine(directory, fileName);
            File.Delete(filePath);
        }

        public static void DeleteAllFilesInDownloads()
        {
            string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            if (Directory.Exists(downloadsPath))
            {
                try
                {
                    foreach (string filePath in Directory.GetFiles(downloadsPath))
                    {
                        File.Delete(filePath);
                    }
                    Console.WriteLine("All files in Downloads folder have been deleted.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while deleting files: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Downloads folder not found.");
            }
        }

    }
}
