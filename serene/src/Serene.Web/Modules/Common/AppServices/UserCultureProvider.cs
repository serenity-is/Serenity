using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace Serene.AppServices;

public class UserCultureProvider : RequestCultureProvider
{
    public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var culture = httpContext.Request.Cookies["LanguagePreference"];
        if (string.IsNullOrEmpty(culture) ||
            culture.Length > 5)
            return NullProviderCultureResult;

        if (culture.Length == 2)
        {
            if (TwoLetterToFourLetter.TryGetValue(culture, out string code))
                culture = code;
            else
                culture = culture + "-" + culture.ToUpperInvariant();
        }

        return Task.FromResult(new ProviderCultureResult(culture));
    }

    private static readonly Dictionary<string, string> TwoLetterToFourLetter =
        new(StringComparer.OrdinalIgnoreCase)
    {
        { "en", "en-US" },
        { "zh", "zh-CN" },
        { "vi", "vi-VN" },
        { "fa", "fa-IR" }
    };

    private static List<CultureInfo> supportedCultures;
    private static readonly string[] supportedCultureIdentifiers = [
        "de-DE",
        "en-US",
        "en-GB",
        "es-ES",
        "fa-IR",
        "it-IT",
        "pt-PT",
        "pt-BR",
        "ru-RU",
        "tr-TR",
        "vi-VN",
        "zh-CN"
    ];

    public static IList<CultureInfo> SupportedCultures
    {
        get => supportedCultures ??= supportedCultureIdentifiers.Select(x =>
        {
            try
            {
                return new CultureInfo(x);
            }
            catch
            {
                return null;
            }
        }).Where(x => x != null).ToList();
    }
}