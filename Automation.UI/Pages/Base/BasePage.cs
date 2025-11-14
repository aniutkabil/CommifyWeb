using Microsoft.Playwright;

namespace Automation.UI.Pages.Base;

public abstract class BasePage(IPage page)
{
    public readonly IPage Page = page;

    private Lazy<ILocator> ErrorMessage => Element(".error");

    protected Lazy<ILocator> Element(string selector) => new(() => Page.Locator(selector));

    public async Task GotoAsync(string url) => await Page.GotoAsync(url);

    public async Task<bool> IsErrorVisibleAsync() => await ErrorMessage.Value.IsVisibleAsync();
}
