namespace Automation.Core.Helpers
{
    public static class TextGenerator
    {
        private static readonly Random _random = new();

        public static string GenerateRandomEmail(string domain = "adnoc.ae")
        {
            var emailName = new string(
                [.. Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz", 10).Select(s => s[_random.Next(s.Length)])]);

            return emailName + "@" + domain;
        }

        public static string GenerateRandomString(int length = 10, string prefix = "")
        {
            var randomString = new string(
                [.. Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz", length).Select(s => s[_random.Next(s.Length)])]);
            return prefix == string.Empty ? randomString : prefix + randomString;
        }

        public static string GenerateWithDefault(int length = 10, string prefix = "يAutotest - ") => GenerateRandomString(length, prefix);

        public static string GenerateRandomNumber(int length = 5)
        {
            var randomString = new string(
                [.. Enumerable.Repeat("0123456789", length).Select(s => s[_random.Next(s.Length)])]);
            return randomString;
        }
    }
}
