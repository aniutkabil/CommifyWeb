using Automation.UI.Pages.Login;
using Microsoft.Playwright;

namespace Automation.UI.Managers.Login
{
    public class LoginManager(IPage page)
    {
        public readonly LoginPage LoginPage = new(page);

        public async Task NavigateToLoginAsync() =>
            await LoginPage.NavigateAsync();

        public async Task LoginAsync(string username, string password) =>
            await LoginPage.LoginAsync(username, password);

        public async Task<bool> IsDashboardVisibleAsync() =>
            await LoginPage.IsDashboardVisibleAsync();
    }
}
    