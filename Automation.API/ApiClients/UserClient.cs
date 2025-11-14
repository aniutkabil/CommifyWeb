using Automation.DataModels.API;
using Automation.Core.DataConfiguration;
using Automation.Core.Helpers;
using RestSharp;

namespace Automation.API.ApiClients
{
    public record UserClient : BaseApiClientContext
    {
        public UserClient(RestClient restClient, HttpClientConfiguration configuration)
            : base(restClient, configuration)
        {
        }

        public async Task<UserDto?> GetUserAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username));

            return await Task.Run(() =>
            {
                var response = MakeRequest(Method.Get, $"/users/{username}", throwEx: true, expectContent: true);
                return JsonHelper.Deserialize<UserDto>(response.Content);
            });
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            return request == null
                ? throw new ArgumentNullException(nameof(request))
                : await Task.Run(() =>
            {
                var response = MakeRequest(Method.Post, "/auth/login", request, throwEx: true, expectContent: true)!;
                return JsonHelper.Deserialize<LoginResponseDto>(response.Content);
            });
        }

        public async Task<bool> LogoutAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            RestClient.AddDefaultHeader("Authorization", $"Bearer {token}");

            return await Task.Run(() =>
            {
                var response = MakeRequest(Method.Post, "/auth/logout", throwEx: true)!;
                return response.IsSuccessful;
            });
        }

        public async Task<List<UserDto>> GetAllUsersAsync(int pageSize = 200, int? maxItems = null)
        {
            return await Task.Run(() =>
                GetAllPaginatedItems<UserDto>(Method.Get, "/users", pageSize: pageSize, maxItemsCount: maxItems)
            );
        }
    }

}