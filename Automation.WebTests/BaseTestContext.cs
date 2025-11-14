using Automation.API.ApiClients;
using Automation.Core.DataConfiguration;
using Automation.UI.Drivers;
using Reqnroll;
using RestSharp;
using System.Reflection;

namespace Automation.WebTests
{
    [Binding]
    public class BaseTestContext
    {
        public BaseApiClientContext? ApiContext { get; private set; }
        public UserClient? UserClient { get; private set; }

        public PlaywrightDriver? Driver { get; set; }

        public HttpClientConfiguration? HttpConfig { get; private set; }
        public UIClientConfiguration? WebConfig { get; private set; }

        public string Username1 { get; private set; } = null!;
        public string Password1 { get; private set; } = null!;
        public string Host { get; private set; } = null!;

        public void Initialize()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var envPath = GetEnvironmentSettingPath();

            DataConfigurationManager.Instance.LoadConfigurations(
                $"Resources.{envPath}.environmentSettings.json",
                assembly,
                variablesFilePath: $"Resources.{envPath}.vars.json"
            );

            HttpConfig = DataConfigurationManager.Instance.GetConfiguration<HttpClientConfiguration>()!;
            WebConfig = DataConfigurationManager.Instance.GetConfiguration<UIClientConfiguration>()!;

            var restClient = new RestClient(HttpConfig.Endpoints["Host"]);
            ApiContext = new BaseApiClientContext(restClient, HttpConfig);
            UserClient = new UserClient(restClient, HttpConfig);

            Username1 = WebConfig.Parameters["TestAutomationUser1_Username"];
            Password1 = WebConfig.Parameters["TestAutomationUser1_Password"];
            Host = WebConfig.Parameters["Host"];
        }

        private static string GetEnvironmentSettingPath()
        {
            var environmentSettingsPath = Environment.GetEnvironmentVariable("TestEnvironmentSettingsPath");
            return string.IsNullOrEmpty(environmentSettingsPath) ? "TestConfigurations.Dev" : $"TestConfigurations.{environmentSettingsPath}";
        }
    }
}
