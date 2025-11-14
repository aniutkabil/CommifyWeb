namespace Automation.Core.Helpers
{
    public static class NumberGenerator
    {
        private static readonly Random _random = new();

        public static int GenerateRandomNumber(int length = 3)
        {
            var randomNumber = new string(
                [.. Enumerable.Repeat("0123456789", length).Select(s => s[_random.Next(s.Length)])]);

            return int.Parse(randomNumber); 
        }
    }
}
