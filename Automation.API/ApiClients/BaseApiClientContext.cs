using Automation.Core.DataConfiguration;
using Automation.Core.Helpers;
using Newtonsoft.Json;
using RestSharp;
using Tams.TestAutomation.Http.Core;
using static Automation.Core.Logging.LoggingProvider;

namespace Automation.API.ApiClients
{
    public record BaseApiClientContext(RestClient RestClient, HttpClientConfiguration Configuration)
    {
        public RestResponse? MakeRequest(Method method, Uri uri, object? json = null, IDictionary<string, string>? queryParams = null, IDictionary<string, string>? formParams = null, string filePath = "")
        {
            return MakeRequest(method, uri.ToString(), json, queryParams, formParams, filePath);
        }

        public RestResponse? MakeRequest(Method method, string uri, object? json = null, IDictionary<string, string>? queryParams = null, IDictionary<string, string>? formParams = null, string filePath = "", bool throwEx = false, bool expectContent = false)
        {
            try
            {
                Instance.Logger.Information($"{method} request to {Uri.UnescapeDataString(uri)}");

                var request = new RestRequest(uri, method);

                if (json != null)
                {
                    var serializedJson = JsonConvert.SerializeObject(json, Formatting.Indented);
                    Instance.Logger.Debug($"Request JSON Body:\n{serializedJson}");
                    request.AddJsonBody(json);
                }

                if (queryParams?.Any() == true)
                {
                    var queryLog = string.Join(", ", queryParams.Select(kv => $"{kv.Key}={kv.Value}"));
                    Instance.Logger.Information("Query Parameters: {QueryParams}", queryLog);
                    foreach (var (key, value) in queryParams)
                    {
                        request.AddQueryParameter(key, value);
                    }
                }

                if (formParams?.Any() == true)
                {
                    var formLog = string.Join(", ", formParams.Select(kv => $"{kv.Key}={kv.Value}"));
                    Instance.Logger.Information("Form Parameters: {FormParams}", formLog);
                    request.AlwaysMultipartFormData = true;
                    foreach (var (key, value) in formParams)
                    {
                        request.AddParameter(key, value);
                    }
                }

                var response = RestClient.Execute(request);

                Instance.Logger.Information("Response Status: {StatusCode} {StatusDescription}",
                    (int)response.StatusCode, response.StatusDescription);

                if (!response.IsSuccessful && throwEx)
                {
                    Instance.Logger.Warning("Request failed with status: {StatusCode} - {StatusDescription}\nContent: {Content}",
                    (int)response.StatusCode, response.StatusDescription, response.Content);

                    throw new HttpRequestException($"Request failed: {(int)response.StatusCode} - {response.StatusDescription}");
                }

                if (expectContent && string.IsNullOrWhiteSpace(response.Content))
                {
                    var message = $"Expected response content but received none for {method} {uri}.";
                    Instance.Logger.Warning(message);
                    throw new InvalidOperationException(message);
                }

                return response;
            }
            catch (Exception ex)
            {
                Instance.Logger.Error(ex, "An error occurred while making the HTTP request to {Uri}", uri);
                throw;
            }
        }

        public PaginatedResponse<T>? GetPaginatedResponse<T>(Method method, Uri uri, object? json = null, IDictionary<string, string>? queryParams = null, IDictionary<string, string>? formParams = null, string filePath = "")
        {
            var response = MakeRequest(method, uri, json, queryParams, formParams, filePath);
            return JsonHelper.Deserialize<PaginatedResponse<T>>(response.Content);
        }

        public PaginatedResponse<T> GetPaginatedResponse<T>(Method method, string uri, object? json = null, IDictionary<string, string>? queryParams = null, IDictionary<string, string>? formParams = null, string filePath = "")
        {
            var response = MakeRequest(method, uri, json, queryParams, formParams, filePath);

            return JsonHelper.Deserialize<PaginatedResponse<T>>(response.Content)!;
        }

        public List<T> GetAllPaginatedItems<T>(Method method, string uri, object? json = null, IDictionary<string, string>? queryParams = null, IDictionary<string, string>? formParams = null, string filePath = "", int pageSize = 200, int? maxItemsCount = null, int pageStep = 1)
        {
            var allItems = new List<T>();
            int currentPage = 1;

            while (true)
            {
                var pagedParams = queryParams != null
                    ? new Dictionary<string, string>(queryParams)
                    : [];

                pagedParams["pageNumber"] = currentPage.ToString();
                pagedParams["pageSize"] = pageSize.ToString();

                var response = MakeRequest(method, uri, json, pagedParams, formParams, filePath);

                var result = JsonHelper.Deserialize<PaginatedResponse<T>>(response.Content)!;

                if (result?.Items == null || !result.Items.Any())
                    break;

                if (maxItemsCount.HasValue && allItems.Count + result.Items.Count() >= maxItemsCount.Value)
                {
                    var remaining = maxItemsCount.Value - allItems.Count;
                    allItems.AddRange(result.Items.Take(remaining));
                    break;
                }

                allItems.AddRange(result.Items);

                if (result.Items.Count() < pageSize)
                    break;

                pageSize = result.Items.Count() > pageSize ? result.Items.Count() / pageStep : pageSize;

                currentPage++;
            }

            return allItems;
        }

    }
}