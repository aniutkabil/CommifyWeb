using Newtonsoft.Json.Linq;

namespace Automation.Core.Configuration
{
	public class ConfigurationManager
	{
		private static readonly Dictionary<string, ConfigurationManager> _instances = [];

		private readonly Dictionary<string, object> _configurationCache;
		private readonly string _configurationJsonString;

		private ConfigurationManager(string jsonFilePath)
		{
			_configurationJsonString = ReadJsonConfiguration(jsonFilePath);
			_configurationCache = [];
		}

		public static ConfigurationManager GetInstance(string jsonFilePath)
		{
			if (!_instances.TryGetValue(jsonFilePath, out ConfigurationManager? value))
			{
				value = new ConfigurationManager(jsonFilePath);
				_instances[jsonFilePath] = value;
			}
			return value;
		}

		private static string ReadJsonConfiguration(string jsonFilePath)
		{
			return !File.Exists(jsonFilePath)
				? throw new FileNotFoundException($"Configuration file '{jsonFilePath}' not found.")
				: File.ReadAllText(jsonFilePath);
		}

		public Dictionary<string, T>? GetConfigAsDictionary<T>(string key)
		{
			if (_configurationCache.TryGetValue(key, out object? value))
			{
				return (Dictionary<string, T>)value;
			}

			var token = JToken.Parse(_configurationJsonString)[key];
			var dictionary = token?.ToObject<Dictionary<string, T>>();

			if (dictionary != null)
			{
				_configurationCache[key] = dictionary;
			}

			return dictionary;
		}
	}

}