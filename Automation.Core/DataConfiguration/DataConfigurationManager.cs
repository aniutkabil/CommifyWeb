using Automation.Core.Helpers;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Automation.Core.DataConfiguration
{
    public sealed class DataConfigurationManager
    {
        private static readonly Lazy<DataConfigurationManager> _instance = new(() => new DataConfigurationManager());

        private readonly Dictionary<Type, BaseConfiguration> _configurations = [];

        public static DataConfigurationManager Instance => _instance.Value;

        private DataConfigurationManager() { }

        public void LoadConfigurations(string envFilePath, Assembly assembly, string appType = "Commify", string? variablesFilePath = null)
        {
            var environmentData = JsonHelper.TryParseJson(ResourceHelper.LoadEmbeddedResource(assembly, envFilePath));

            var variables = variablesFilePath is not null
                ? JsonHelper.TryParseJson(ResourceHelper.LoadEmbeddedResource(assembly, variablesFilePath))?
                .ToObject<Dictionary<string, string>>() ?? [] : [];

            if (environmentData != null)
            {
                SetConfiguration<HttpClientConfiguration>(environmentData, "HttpClient", appType, variables);
                SetConfiguration<UIClientConfiguration>(environmentData, "UI", appType, variables);
                SetConfiguration<DatabaseConfiguration>(environmentData, "Databases", "Sql", variables);
            }
            else
            {
                throw new InvalidOperationException($"Resource {envFilePath} is null and cannot be processed.");
            }
        }

        public void SetConfiguration<T>(string resourceName, Assembly assembly, string key, string appType = "Commify") where T : BaseConfiguration
        {
            string jsonContent = ResourceHelper.LoadEmbeddedResource(assembly, resourceName);
            var jsonNode = JsonHelper.TryParseJson(jsonContent);
            if (jsonNode != null)
            {
                SetConfiguration<T>(jsonNode, key, appType);
            }
            else
            {
                throw new InvalidOperationException($"Resource {resourceName} is null and cannot be processed.");
            }
        }

        private void SetConfiguration<T>(JObject jsonNode, string key, string subKey, Dictionary<string, string>? variables = null) where T : BaseConfiguration
        {
            var section = JsonHelper.GetJsonNodeSection<T>(jsonNode, key, subKey)
                ?? throw new InvalidOperationException($"No data found for '{key}:{subKey}'.");

            if (variables != null)
            {
                section.ResolvePlaceholders(variables);
            }

            _configurations[typeof(T)] = section;
        }

        public T? GetConfiguration<T>() where T : BaseConfiguration
        {
            return _configurations.TryGetValue(typeof(T), out var config) ? config as T : null;
        }
    }

}