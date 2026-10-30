using Microsoft.Playwright.TestAdapter;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace Serenity.Testing.Playwright;

/// <summary>
/// Temporary workaround from https://github.com/microsoft/playwright-dotnet/issues/3081
/// </summary>
public class PlaywrightSettings
{
    public string Browser { get; set; } = "Chromium";
    public BrowserTypeLaunchOptions LaunchOptions { get; set; }
    public TimeSpan? ExpectTimeout { get; set; }

    static bool initialized;

    public static void Initialize()
    {
        if (initialized) return;
        initialized = true;

        new PlaywrightSettings()
        {
            LaunchOptions = new()
            {
                Headless = Environment.OSVersion.Platform != PlatformID.Win32NT
            }
        }.Install();
    }

    public void Install()
    {
        using var reader = CreateXmlReader();
        new PlaywrightSettingsProvider().Load(reader);
    }

    private XmlReader CreateXmlReader()
    {
        var playwright = new XElement("Playwright", new XElement("BrowserName", Browser));
        if (LaunchOptions != null)
        {
            playwright.Add(CreateLaunchOptions(LaunchOptions));
        }

        if (ExpectTimeout != null)
        {
            playwright.Add(new XElement(nameof(ExpectTimeout), ExpectTimeout.Value.TotalMilliseconds));
        }

        return playwright.CreateReader();
    }

    private static XElement CreateLaunchOptions(BrowserTypeLaunchOptions options)
    {
        var launchOptions = new XElement(nameof(LaunchOptions));
        if (options.Args != null) launchOptions.Add(new XElement(nameof(options.Args)), JsonSerializer.Serialize(options.Args));
        if (options.Channel != null) launchOptions.Add(new XElement(nameof(options.Channel)), options.Channel);
        if (options.ChromiumSandbox != null) launchOptions.Add(new XElement(nameof(options.ChromiumSandbox)), options.ChromiumSandbox);
        if (options.DownloadsPath != null) launchOptions.Add(new XElement(nameof(options.DownloadsPath)), options.DownloadsPath);
        if (options.Env != null) launchOptions.Add(new XElement(nameof(options.Env)), JsonSerializer.Serialize(options.Env.ToDictionary(x => x.Key)));
        if (options.ExecutablePath != null) launchOptions.Add(new XElement(nameof(options.ExecutablePath)), options.ExecutablePath);
        if (options.FirefoxUserPrefs != null) launchOptions.Add(new XElement(nameof(options.FirefoxUserPrefs)), JsonSerializer.Serialize(options.FirefoxUserPrefs.ToDictionary(x => x.Key)));
        if (options.HandleSIGHUP != null) launchOptions.Add(new XElement(nameof(options.HandleSIGHUP)), options.HandleSIGHUP);
        if (options.HandleSIGINT != null) launchOptions.Add(new XElement(nameof(options.HandleSIGINT)), options.HandleSIGINT);
        if (options.HandleSIGTERM != null) launchOptions.Add(new XElement(nameof(options.HandleSIGTERM)), options.HandleSIGTERM);
        if (options.Headless != null) launchOptions.Add(new XElement(nameof(options.Headless)), options.Headless);
        if (options.IgnoreAllDefaultArgs != null) launchOptions.Add(new XElement(nameof(options.IgnoreAllDefaultArgs)), options.IgnoreAllDefaultArgs);
        if (options.IgnoreDefaultArgs != null) launchOptions.Add(new XElement(nameof(options.IgnoreDefaultArgs)), options.IgnoreDefaultArgs);
        if (options.Proxy?.Server != null) launchOptions.Add(new XElement(nameof(options.Proxy)), CreateProxy(options.Proxy));
        if (options.SlowMo != null) launchOptions.Add(new XElement(nameof(options.SlowMo)), options.SlowMo);
        if (options.Timeout != null) launchOptions.Add(new XElement(nameof(options.Timeout)), options.Timeout);
        if (options.TracesDir != null) launchOptions.Add(new XElement(nameof(options.TracesDir)), options.TracesDir);
        return launchOptions;
    }

    private static XElement CreateProxy(Proxy value)
    {
        var proxy = new XElement(nameof(Proxy), new XAttribute(nameof(value.Server), value.Server));
        if (value.Bypass != null) proxy.Add(new XElement(nameof(value.Bypass), value.Bypass));
        if (value.Username != null) proxy.Add(new XElement(nameof(value.Username), value.Username));
        if (value.Password != null) proxy.Add(new XElement(nameof(value.Password), value.Password));
        return proxy;
    }
}