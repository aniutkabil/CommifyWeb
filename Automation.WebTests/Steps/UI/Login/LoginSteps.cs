using Automation.Core.Reporting.Services;
using Automation.UI.Managers.Login;
using Automation.UI.Routing;
using Automation.WebTests.Steps.Shared;
using FluentAssertions;
using Reqnroll;

namespace Automation.WebTests.Steps.UI.Login
{
    [Binding, Scope(Tag = "ui")]
    public class LoginSteps(ScenarioContext scenarioContext, FeatureContext featureContext, SoftAssertService softAssert) : BaseWebSteps(scenarioContext, featureContext, softAssert)
    {
        private LoginManager? _loginManager;

        private LoginManager LoginManager =>
            _loginManager ??= new LoginManager(Driver!.Page!);

        [Given(@"valid user credentials are already registered")]
        public void GivenValidUserCredentialsAreRegistered()
        {
            //this was added since step was pre-defined in task description
            //However, Username and Password can be used directly from DataConfigurationManager instance available in BaseWebSteps.
            ScenarioContext["Username"] = WebConfig.Parameters["TestAutomationUser1_Username"];
            ScenarioContext["Password"] = WebConfig.Parameters["TestAutomationUser1_Password"];
        }

        [Given(@"I’m on the login screen")]
        public async Task GivenIAmOnTheLoginPage()
        {
            await LoginManager.NavigateToLoginAsync();
        }

        [Given(@"I’m not logged in with a genuine user")]
        public async Task GivenINotLoggedIn()
        {
            await Driver!.Page.Context.ClearCookiesAsync();
        }

        [When(@"I navigate to any page on the tracking site")]
        public async Task WhenINavigateToAnyPage()
        {
            await Driver!.Page.GotoAsync($"{Host}{PageUrls.Dashboard}");
        }

        [When(@"I enter a valid username and password and submit")]
        public async Task WhenILogIn()
        {
            await LoginManager.LoginAsync(Username, Password);
        }

        [Then(@"I am logged in successfully")]
        public async Task ThenDashboardVisible()
        {
            var title = await LoginManager.LoginPage.Page!.TitleAsync();

            SoftAssert.That(() => title.Should().Contain("Dashboard", "after successful login, the dashboard should be visible"));
        }

        [Then(@"I am presented with a login screen")]
        public async Task ThenLoginScreenVisible()
        {
            var isVisible = await LoginManager.LoginPage.Username.Value.IsVisibleAsync();

            SoftAssert.That(() => isVisible.Should().BeTrue("login screen should be visible for not logged-in user"));
        }
    }
}
