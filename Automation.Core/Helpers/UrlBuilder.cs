namespace Automation.Core.Helpers
{
	public class UrlBuilder(string baseUri)
    {
        private readonly Dictionary<string, string> _queryParams = [];
        private readonly List<string> _filters = [];

        public UrlBuilder AddParameter(string key, string value)
        {
            _queryParams[key] = value;
            return this;
        }

        public UrlBuilder AddFilter(string filter)
        {
            _filters.Add(filter);
            return this;
        }

        public string Build()
        {
            List<string> parts = [];

            if (_queryParams.Count != 0)
            {
                parts.Add(string.Join("&", _queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}")));
            }
            if (_filters.Count != 0)
            {
                parts.Add($"filter={Uri.EscapeDataString(string.Join(", ", _filters))}");
            }
            return baseUri + (parts.Count != 0 ? "?" + string.Join("&", parts) : string.Empty);
        }
    }
}
