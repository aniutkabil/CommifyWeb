using System.Collections.Concurrent;
using System.Reflection;

namespace Automation.Core.Helpers
{
	public static class ResourceHelper
	{
		private static readonly ConcurrentDictionary<string, Assembly> _assemblies = new();

		public static void RegisterAssembly(string assemblyName, Assembly assembly)
		{
			_assemblies.TryAdd(assemblyName, assembly);
		}

		/// <summary>
		/// Load embedded resource from the assembly.
		/// </summary>
		/// <param name="assembly">Assembly that contains resource.</param>
		/// <param name="resourceName">Name of the resource (file name).</param>
		/// <returns>Resource as a string</returns>
		/// <exception cref="FileNotFoundException"></exception>
		public static string LoadEmbeddedResource(Assembly assembly, string resourceName)
		{
			var assemblyName = assembly.GetName().Name;
			var resourcePath = $"{assemblyName}.{resourceName}";
			using var stream = assembly.GetManifestResourceStream(resourcePath);

			if (stream == null)
			{
				throw new FileNotFoundException($"Resource '{resourceName}' not found in assembly '{assemblyName}'.");
			}

			using var reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}

		public static string LoadEmbeddedResource(string resourceName, string assemblyName)
		{
			if (!_assemblies.TryGetValue(assemblyName, out var assembly))
			{
				throw new InvalidOperationException($"Assembly '{assemblyName}' has not been registered.");
			}

			var resourcePath = $"{assembly.GetName().Name}.{resourceName}";
			using var stream = assembly.GetManifestResourceStream(resourcePath);
			if (stream == null)
			{
				throw new FileNotFoundException($"Resource '{resourceName}' not found in assembly '{assemblyName}'.");
			}

			using var reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}
	}
}