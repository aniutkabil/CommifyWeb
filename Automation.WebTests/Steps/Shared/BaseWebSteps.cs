using Automation.Core.DataConfiguration;
using Automation.Core.Reporting.Services;
using Reqnroll;

namespace Automation.WebTests.Steps.Shared
{
    public class BaseWebSteps(ScenarioContext scenarioContext, FeatureContext testContext, SoftAssertService softAssert) : BaseSteps(scenarioContext, testContext, softAssert)
    {
        protected static UIClientConfiguration WebConfig = DataConfigurationManager.Instance.GetConfiguration<UIClientConfiguration>()!;

        protected static string Username = WebConfig.Parameters["TestAutomationUser1_Username"];
        protected static string Password = WebConfig.Parameters["TestAutomationUser1_Password"];
        protected static string Host = WebConfig.Parameters["Host"];
    }
}
