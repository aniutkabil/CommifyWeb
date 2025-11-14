using Automation.Core.Reporting.Services;
using Automation.UI.Drivers;
using Reqnroll;

namespace Automation.WebTests.Hooks
{
    [Binding]
    public sealed class TestHooks
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;
        private readonly SoftAssertService _softAssert;
        private BaseTestContext _testContext;

        public TestHooks(ScenarioContext scenarioContext, FeatureContext featureContext, SoftAssertService softAssert)
        {
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
            _softAssert = softAssert;
        }

        [BeforeScenario(Order = 10)]
        public async Task SetupUI()
        {
            if (!_scenarioContext.ScenarioInfo.Tags.Contains("ui"))
                return;

            _testContext = GetTestContext(_featureContext);

            var headless = bool.Parse(_testContext.WebConfig!.Parameters["Headless"]);

            _testContext.Driver = new PlaywrightDriver(headless);
            await _testContext.Driver.InitializeAsync();
        }

        [BeforeFeature]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            var testContext = new BaseTestContext();
            testContext.Initialize();

            featureContext["TestContext"] = testContext;
        }

        public static BaseTestContext GetTestContext(FeatureContext featureContext)
            => (BaseTestContext)featureContext["TestContext"];

        [AfterScenario(Order = 10)]
        public async Task CleanupUI()
        {
            if (_testContext.Driver != null)
            {
                await _testContext.Driver.DisposeAsync();
                _testContext.Driver = null;
            }
        }

        [AfterScenario(Order = 20)]
        public void AfterScenario()
        {
            _softAssert.AssertAll();
        }
    }
}