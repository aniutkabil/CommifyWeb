using System.Globalization;
using System.Text.RegularExpressions;

namespace Automation.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool ToBoolFromYesNo(this string? value) => value switch
        {
            "Yes" => true,
            "No" => false,
            _ => false
        };

        public static string GetJoinedBySeparator(IEnumerable<string> strings, char separator)
        {
            return string.Join(separator, strings).Trim();
        }

        public static DateTime? TryParseNullableDate(this string dateString, string dateFormat = "dd/MM/yyyy", CultureInfo culture = null)
        {
            culture ??= CultureInfo.InvariantCulture;

            if (DateTime.TryParseExact(dateString, dateFormat, culture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }

            return null;
        }

        public static string NormalizeSpaces(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return Regex.Replace(input.Trim(), @"\s+", " ");
        }
    }
}
