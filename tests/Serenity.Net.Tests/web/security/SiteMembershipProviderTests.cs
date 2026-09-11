namespace Serenity.Web.Providers;

public class SiteMembershipProviderTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ComputeSHA512_Throws_When_String_Is_Null_Or_Empty(string? value)
    {
        Assert.Throws<ArgumentNullException>(() => SiteMembershipProvider.ComputeSHA512(value!));
    }

    [Fact]
    public void ComputeSHA512_Returns_86_Character_Base64_Hash()
    {
        var hash = SiteMembershipProvider.ComputeSHA512("test");

        Assert.Equal(86, hash.Length);
        Assert.DoesNotContain("=", hash);
        Assert.Equal(SiteMembershipProvider.ComputeSHA512("test"), hash);
    }

    [Fact]
    public void ComputeSHA512_Returns_Known_Hash()
    {
        var expected = Convert.ToBase64String(
            System.Security.Cryptography.SHA512.HashData(
                Encoding.UTF8.GetBytes("admin")))[..86];

        Assert.Equal(expected, SiteMembershipProvider.ComputeSHA512("admin"));
    }

    [Fact]
    public void ComputeSHA512_Different_Inputs_Produce_Different_Hashes()
    {
        Assert.NotEqual(
            SiteMembershipProvider.ComputeSHA512("one"),
            SiteMembershipProvider.ComputeSHA512("two"));
    }
}
