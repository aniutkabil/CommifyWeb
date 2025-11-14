using System.Globalization;

namespace Automation.Core.Extensions
{
    public static class DateTimeExtensions
    {
        private const string DefaultDateFormat = "dd/MM/yyyy";

        public static string FormatDate(this DateTime? date, string? dateFormat = null, IFormatProvider? provider = null)
        {
            if (date.HasValue)
            {
                var format = string.IsNullOrEmpty(dateFormat) ? DefaultDateFormat : dateFormat;
                return date.Value.ToString(format, provider ?? CultureInfo.InvariantCulture);
            }

            return string.Empty;
        }
    }
}
