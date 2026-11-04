using System.Globalization;

namespace Serenity.Extensions;

public class BaseUserRetrieveServiceNormalizeUsernameTests
{
    [Theory]
    [InlineData("admin", "ADMIN")]
    [InlineData("Admin", "ADMIN")]
    [InlineData("ADMIN", "ADMIN")]
    [InlineData("isik", "ISIK")]
    [InlineData("ISIK", "ISIK")]
    [InlineData("ışık", "IŞIK")]
    [InlineData("IŞIK", "IŞIK")]
    [InlineData("İŞİK", "IŞIK")]
    [InlineData("user@example.com", "USER@EXAMPLE.COM")]
    public void Folds_Case_And_Turkish_I_Variants(string username, string expected)
    {
        Assert.Equal(expected, BaseUserRetrieveService.NormalizeUsername(username));
    }

    [Fact]
    public void Does_Not_Depend_On_Current_Culture()
    {
        var culture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR");
            Assert.Equal("ADMIN", BaseUserRetrieveService.NormalizeUsername("admin"));
        }
        finally
        {
            CultureInfo.CurrentCulture = culture;
        }
    }

    [Fact]
    public void Composes_Unicode_Before_Folding()
    {
        // I + combining dot above is the decomposed form of İ
        Assert.Equal("I", BaseUserRetrieveService.NormalizeUsername("İ"));
    }

    [Fact]
    public void Throws_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() => BaseUserRetrieveService.NormalizeUsername(null!));
    }
}
