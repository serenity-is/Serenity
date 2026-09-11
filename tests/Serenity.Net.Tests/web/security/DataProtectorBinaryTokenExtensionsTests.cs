using Microsoft.AspNetCore.DataProtection;

namespace Serenity;

public class DataProtectorBinaryTokenExtensionsTests
{
    private static IDataProtector CreateProtector()
    {
        return new EphemeralDataProtectionProvider().CreateProtector("test");
    }

    [Fact]
    public void ProtectBinary_Throws_When_Callback_Is_Null()
    {
        var protector = CreateProtector();
        Assert.Throws<ArgumentNullException>(() => protector.ProtectBinary(null!));
    }

    [Fact]
    public void UnprotectBinary_Throws_When_Token_Is_Null()
    {
        var protector = CreateProtector();
        Assert.Throws<ArgumentNullException>(() => protector.UnprotectBinary(null!));
    }

    [Fact]
    public void ProtectBinary_And_UnprotectBinary_RoundTrip_Values()
    {
        var protector = CreateProtector();

        var token = protector.ProtectBinary(writer =>
        {
            writer.Write(42);
            writer.Write("hello");
        });

        Assert.NotNull(token);
        using var reader = protector.UnprotectBinary(token);
        Assert.Equal(42, reader.ReadInt32());
        Assert.Equal("hello", reader.ReadString());
    }

    [Fact]
    public void ProtectBinary_Returns_Url_Safe_Token()
    {
        var protector = CreateProtector();

        var token = protector.ProtectBinary(writer => writer.Write("some value"));

        Assert.DoesNotContain("+", token);
        Assert.DoesNotContain("/", token);
    }
}
