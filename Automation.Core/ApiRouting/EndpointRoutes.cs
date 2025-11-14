using Automation.Core.DataConfiguration;

namespace Automation.Core.ApiRouting
{
    public static class EndpointRoutes
    {
        private static HttpClientConfiguration _httpClientConfiguration = null!;

        public static void Initialize(HttpClientConfiguration httpClientConfiguration) => _httpClientConfiguration = httpClientConfiguration;

        public static class UserRequests
        {
            public static string SubmitUrl => $"{_httpClientConfiguration.Endpoints["Users"]}";
        }
    }
}
