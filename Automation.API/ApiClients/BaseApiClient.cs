using System.Net.Http.Json;
using System.Text.Json;

namespace Automation.API.ApiClients
{
    public abstract class BaseApiClient
    {
        protected readonly HttpClient HttpClient;

        protected BaseApiClient(string baseUrl)
        {
            HttpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        protected async Task<T?> GetAsync<T>(string relativeUrl)
        {
            var response = await HttpClient.GetAsync(relativeUrl);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
        }

        protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string relativeUrl, TRequest payload)
        {
            var response = await HttpClient.PostAsJsonAsync(relativeUrl, payload);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TResponse>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
        }
    }
}
