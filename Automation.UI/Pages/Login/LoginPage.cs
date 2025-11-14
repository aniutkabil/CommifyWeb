using Automation.Core.DataConfiguration;
using Automation.UI.Pages.Base;
using Automation.UI.Routing;
using Microsoft.Playwright;

namespace Automation.UI.Pages.Login;

public class LoginPage(IPage page) : BasePage(page)
{
    public Lazy<ILocator> Username => Element("#username");
    public Lazy<ILocator> Password => Element("#password");
    public Lazy<ILocator> SubmitBtn => Element("button[type='submit']");

    public async Task LoginAsync(string user, string pass)
    {
        await Username.Value.FillAsync(user);
        await Password.Value.FillAsync(pass);
        await SubmitBtn.Value.ClickAsync();
    }

    public async Task NavigateAsync() => await Page.GotoAsync($"{DataConfigurationManager.Instance.GetConfiguration<UIClientConfiguration>()!.Parameters["Host"]}{PageUrls.Login}");

    public async Task<bool> IsDashboardVisibleAsync()
    {
        var title = await Page.TitleAsync();
        return title.Contains("Dashboard", StringComparison.OrdinalIgnoreCase);
    }
}
