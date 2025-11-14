using NewtonSoftJson = Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Automation.Core.Helpers
{
    public static class JsonHelper
    {
        public static T? ReadJsonFile<T>(string relativeFilePath, JsonSerializerOptions? options = null)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(currentDirectory, relativeFilePath);

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File by path '{filePath}' was not found.");

            return Deserialize<T>(File.ReadAllText(filePath), options);
        }

        public static T DeserializeHttpResponse<T>(string? content)
        {
            return content is null
                ? throw new ArgumentNullException(nameof(content), "Content cannot be null.")
                : NewtonSoftJson.JsonConvert.DeserializeObject<T>(content)
                ?? throw new InvalidOperationException($"Deserialization failed for type {typeof(T).Name}. Ensure the JSON content is valid.");
        }

        public static T? ReadDataFromJsonFile<T>(string relativeFilePath, JsonSerializerOptions? options = null, params string[] dataPath)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(currentDirectory, relativeFilePath);

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File by path '{filePath}' was not found.");

            var json = File.ReadAllText(filePath);

            JsonNode? jsonNode = JsonNode.Parse(json);

            foreach (var segment in dataPath)
            {
                if (jsonNode?[segment] != null)
                {
                    jsonNode = jsonNode[segment];
                }
                else
                {
                    throw new InvalidOperationException($"Path '{string.Join(".", dataPath)}' not found in the JSON.");
                }
            }

            return jsonNode.Deserialize<T>(options ?? ReadOptions);
        }

        public static async Task<T?> ReadJsonFileAsync<T>(this string filePath, JsonSerializerOptions? options = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath), $"File path cannot be null or empty.");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File by path '{filePath}' was not found.");

            await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                return await JsonSerializer.DeserializeAsync<T>(stream, options);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to deserialize JSON from {filePath}.", ex);
            }
        }

        private static readonly JsonSerializerOptions _writeOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        private static readonly JsonSerializerOptions _readOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public static JsonSerializerOptions ReadOptions => _readOptions;

        public static JsonSerializerOptions WriteOptions => _writeOptions;

        public static string Serialize<T>(T value, JsonSerializerOptions? options = null)
        {
            try
            {
                return JsonSerializer.Serialize(value, options ?? WriteOptions);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to serialize.", ex);
            }
        }

        public static T? Deserialize<T>(string json, JsonSerializerOptions? options = null)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json, options ?? ReadOptions);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to deserialize.", ex);
            }
        }

        public static T? Deserialize<T>(JsonNode jsonNode, JsonSerializerOptions? options = null)
        {
            try
            {
                return jsonNode.Deserialize<T>(options ?? ReadOptions);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to deserialize.", ex);
            }
        }

        public static T? GetJsonNodeSection<T>(JObject jsonNode, string key, string subkey)
        {
            var token = jsonNode.SelectToken($"{key}.{subkey}") ?? throw new Exception($"Node '{key}:{subkey}' not found in JSON.");
           
            return token.ToObject<T>();
        }

        public static JObject? TryParseJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("JSON can't be null or empty.", nameof(json));
            }

            try
            {
                return JObject.Parse(json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse the JSON file: {ex.Message}", ex);
            }
        }
    }
}
