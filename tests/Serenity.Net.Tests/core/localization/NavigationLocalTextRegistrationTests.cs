using Serenity.Navigation;

namespace Serenity.Localization;

public class NavigationLocalTextRegistrationTests
{
    [Fact]
    public void AddNavigationTexts_ThrowsArgumentNullException_WhenTypeSourceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new MockLocalTextRegistry().AddNavigationTexts(null));
    }

    [Fact]
    public void AddNavigationTexts_ThrowsArgumentNullException_WhenRegistryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ((ILocalTextRegistry)null).AddNavigationTexts(new MockTypeSource()));
    }

    [Fact]
    public void AddNavigationTexts_AddsTextsForNavigationItems()
    {
        var registry = new MockLocalTextRegistry();
        var typeSource = new MockTypeSource([
            new NavigationMenuAttribute(1, "Dashboard"),
            new NavigationMenuAttribute(2, "Administration/Users")
        ]);

        registry.AddNavigationTexts(typeSource);

        Assert.Contains((LocalText.InvariantLanguageID, "Navigation.Dashboard", "Dashboard"), registry.AddedList);
        Assert.Contains((LocalText.InvariantLanguageID, "Navigation.Administration/Users", "Users"), registry.AddedList);
    }

    [Fact]
    public void AddNavigationTexts_UsesSpecifiedLanguage()
    {
        var registry = new MockLocalTextRegistry();
        var typeSource = new MockTypeSource([new NavigationMenuAttribute(1, "Dashboard")]);

        registry.AddNavigationTexts(typeSource, "tr");

        Assert.Contains(("tr", "Navigation.Dashboard", "Dashboard"), registry.AddedList);
    }
}
