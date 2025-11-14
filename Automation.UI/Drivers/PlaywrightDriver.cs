using Automation.Core.DataConfiguration;
using Automation.UI.Pages.Base;
using Microsoft.Playwright;

namespace Automation.UI.Drivers
{
    public class PlaywrightDriver(bool headless) : IAsyncDisposable
    {
        private bool _disposed;

        private readonly DataConfigurationManager _dataConfig = DataConfigurationManager.Instance!;

        public IPlaywright Playwright { get; private set; } = null!;
        public IBrowser Browser { get; private set; } = null!;
        public IBrowserContext Context { get; private set; } = null!;
        public IPage Page { get; private set; } = null!;

        public async Task InitializeAsync()
        {
            Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

            Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = headless,
                Args = ["--start-maximized"]
            });

            Context = await Browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = null
            });

            Page = await Context.NewPageAsync();
        }

        public TPage CreatePage<TPage>() where TPage : BasePage
        {
            return (TPage)Activator.CreateInstance(typeof(TPage), Page, _dataConfig)!;
        }

        public async Task GotoAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));

            await Page.GotoAsync(url);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            try
            {
                if (Page != null)
                    await Page.CloseAsync();

                if (Context != null)
                    await Context.CloseAsync();

                if (Browser != null)
                    await Browser.CloseAsync();
            }
            finally
            {
                Playwright?.Dispose();
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        public void Dispose() => DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
}