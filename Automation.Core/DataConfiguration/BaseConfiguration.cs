using System.Text.RegularExpressions;

namespace Automation.Core.DataConfiguration
{
    public abstract class BaseConfiguration
    {
        public virtual Dictionary<string, string> Parameters { get; set; } = [];

        public void ResolvePlaceholders(Dictionary<string, string> variables)
        {
            Parameters = Parameters.ToDictionary(
                kvp => kvp.Key,
                kvp => ResolveValue(kvp.Value, variables)
            );
        }

        private static string ResolveValue(string value, Dictionary<string, string> context, HashSet<string>? visited = null)
        {
            visited ??= [];

            return Regex.Replace(value, @"\$\(([^)]+)\)", match =>
            {
                var key = match.Groups[1].Value;
                if (!context.TryGetValue(key, out var replacement))
                    throw new InvalidOperationException($"Variable '{key}' not found for placeholder replacement.");

                if (visited.Contains(key))
                    throw new InvalidOperationException($"Circular reference detected for key '{key}'");

                visited.Add(key);
                return ResolveValue(replacement, context, visited);
            });
        }
    }

}