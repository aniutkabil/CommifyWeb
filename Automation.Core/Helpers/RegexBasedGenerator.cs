using Fare;

namespace Automation.Core.Helpers
{
    public static class RegexBasedGenerator
    {
        private static readonly Random _random = new();

        public static string Generate(string regexPattern)
        {
            var xeger = new Xeger(regexPattern, _random);
            return xeger.Generate();
        }
    }
}
