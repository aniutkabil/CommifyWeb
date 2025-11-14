using Automation.Core.Reporting.Services;
using Automation.UI.Drivers;
using Automation.WebTests;
using Reqnroll;

namespace Automation.WebTests.Steps.Shared
{
    public abstract class BaseSteps
    {
        protected readonly ScenarioContext ScenarioContext;
        protected readonly FeatureContext FeatureContext;
        protected readonly SoftAssertService SoftAssert;

        protected BaseSteps(
            ScenarioContext scenarioContext,
            FeatureContext featureContext,
            SoftAssertService softAssert)
        {
            ScenarioContext = scenarioContext;
            FeatureContext = featureContext;
            SoftAssert = softAssert;
        }

        protected BaseTestContext TestContext
            => (BaseTestContext)FeatureContext["TestContext"];

        protected PlaywrightDriver Driver =>
            TestContext.Driver
            ?? throw new InvalidOperationException("Driver is not initialized. Ensure this scenario has 'ui' tag.");

        protected T GetApiClient<T>(Func<BaseTestContext, T?> selector) where T : class
        {
            var client = selector(TestContext);
            return client ?? throw new InvalidOperationException($"{typeof(T).Name} not initialized.");
        }

        protected void SetScenarioData<T>(string key, T value)
            => ScenarioContext[key] = value;

        protected T GetScenarioData<T>(string key)
        {
            if (!ScenarioContext.TryGetValue(key, out var obj) || obj is not T typed)
                throw new InvalidOperationException($"Scenario key '{key}' does not exist or has wrong type.");

            return typed;
        }
    }
}