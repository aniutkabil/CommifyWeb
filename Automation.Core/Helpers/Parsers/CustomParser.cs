namespace Automation.Core.Helpers.Parsers
{
    public static class CustomParser
    {
        public static TEnum[] TryParseEnum<TEnum>(string param)
            where TEnum : struct, Enum =>
            [.. param.Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Where(s => Enum.TryParse<TEnum>(s, true, out _))
                .Select(s => Enum.Parse<TEnum>(s, ignoreCase: true))];

        public static int[] ParseEnumToInts<TEnum>(string param)
            where TEnum : struct, Enum =>
            [.. param
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => Convert.ToInt32(Enum.Parse<TEnum>(s, ignoreCase: true)))];

        public static string ParseEnumToIntString<TEnum>(string filterParam, char delimiter = '|')
            where TEnum : struct, Enum =>
            string.Join(delimiter, filterParam.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => Convert.ToInt32(Enum.Parse<TEnum>(s, ignoreCase: true)))
                );

        public static string ParseEnumToIntString<TEnum>(TEnum[] enums, char delimiter = '|')
            where TEnum : struct, Enum =>
            string.Join(delimiter, enums.Select(e => Convert.ToInt32(e)));

        public static SortInfo ParseSort(string sortParam) => new()
        {
            Field = sortParam.TrimStart('-'),
            Ascending = !sortParam.StartsWith('-')
        };
    }
}
