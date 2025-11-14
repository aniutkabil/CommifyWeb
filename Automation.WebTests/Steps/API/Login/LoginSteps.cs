using Automation.Core.Reporting.Services;
using Automation.DataModels.API;
using Automation.WebTests.Steps.Shared;
using FluentAssertions;
using Reqnroll;

namespace Automation.WebTests.Steps.API.Login
{
    [Binding, Scope(Tag = "api")]
    public class UserSteps(ScenarioContext scenarioContext, FeatureContext featureContext, SoftAssertService softAssert) : BaseSteps(scenarioContext, featureContext, softAssert)
    {
        private const string LoginResponseKey = "LoginResponse";
        private const string UserProfileKey = "UserProfile";

        [Given(@"API user client is available")]
        public void GivenApiUserClientAvailable()
        {
            SoftAssert.That(() =>
                TestContext.UserClient.Should().NotBeNull("UserClient must be initialized in TestHooks")
            );
        }

        [When(@"I log in via API with username '([^']*)' and password '([^']*)'")]
        public async Task WhenILogInViaApi(string username, string password)
        {
            var request = new LoginRequestDto
            {
                Username = username,
                Password = password
            };

            var loginResponse = await TestContext.UserClient!.LoginAsync(request);

            SoftAssert.That(() => loginResponse.Should().NotBeNull("Login API should return a valid response"));

            SetScenarioData(LoginResponseKey, loginResponse);
        }

        [Then(@"the API login should be successful")]
        public void ThenApiLoginShouldBeSuccessful()
        {
            var response = GetScenarioData<LoginResponseDto>(LoginResponseKey);

            SoftAssert.That(() =>
                response.Should().NotBeNull("Login response should exist in scenario context")
            );

            SoftAssert.That(() =>
                response!.Success.Should().BeTrue($"Login should succeed. Message: {response.Message}")
            );
        }

        [Then(@"the API token should not be empty")]
        public void ThenApiTokenShouldNotBeEmpty()
        {
            var response = GetScenarioData<LoginResponseDto>(LoginResponseKey);

            SoftAssert.That(() =>
                response!.Token.Should().NotBeNullOrWhiteSpace("API login should return a non-empty token")
            );
        }

        [When(@"I fetch user profile via API for username '([^']*)'")]
        public async Task WhenIFetchUserProfile(string username)
        {
            var user = await TestContext.UserClient!.GetUserAsync(username);

            SoftAssert.That(() => user.Should().NotBeNull($"User profile for '{username}' should exist"));

            SetScenarioData(UserProfileKey, user);
        }

        [Then(@"the returned user profile should have username '([^']*)'")]
        public void ThenUserProfileShouldHaveUsername(string expectedUsername)
        {
            var user = GetScenarioData<UserDto>(UserProfileKey);

            SoftAssert.That(() => user.Should().NotBeNull("User profile must be present in scenario context"));
            SoftAssert.That(() => user!.Username.Should().BeEquivalentTo(expectedUsername, "Returned username should match expected username"));

            // Optionally currently added assert all to collect all soft failures at the end of the scenario.
            SoftAssert.AssertAll();
        }
    }
}
