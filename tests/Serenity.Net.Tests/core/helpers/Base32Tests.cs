using System.Security.Cryptography;

namespace Serenity.Data;

public class Base32Tests
{
    // from https://github.com/dotnet/aspnetcore/blob/main/src/Identity/test/Identity.Test/Base32Test.cs

    [Fact]
    public void ConversionTest()
    {
        var data = new byte[] { 1, 2, 3, 4, 5, 6 };

        Assert.Equal<byte>(data, Base32.Decode(Base32.Encode(data)));

        int length;
        do
        {
            length = GetRandomByteArray(1)[0];
        } while (length % 5 == 0);
        data = GetRandomByteArray(length);
        Assert.Equal<byte>(data, Base32.Decode(Base32.Encode(data)));

        length = GetRandomByteArray(1)[0] * 5;
        data = GetRandomByteArray(length);
        Assert.Equal<byte>(data, Base32.Decode(Base32.Encode(data)));
    }

    private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();

    private static byte[] GetRandomByteArray(int length)
    {
        byte[] bytes = new byte[length];
        rng.GetBytes(bytes);
        return bytes;
    }

    [Fact]
    public void Encode_ThrowsArgumentNullException_WhenBytesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => Base32.Encode(null));
    }

    [Fact]
    public void Decode_ThrowsArgumentNullException_WhenBase32IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => Base32.Decode(null));
    }

    [Fact]
    public void Decode_ThrowsArgumentException_ForInvalidCharacter()
    {
        Assert.Throws<ArgumentException>(() => Base32.Decode("!"));
        Assert.Throws<ArgumentException>(() => Base32.Decode("0"));
        Assert.Throws<ArgumentException>(() => Base32.Decode("1"));
    }

    [Theory]
    [InlineData("a")]      // length % 8 == 1
    [InlineData("abc")]    // length % 8 == 3
    [InlineData("abcdef")] // length % 8 == 6
    public void Decode_ThrowsArgumentException_ForNonCanonicalLength(string value)
    {
        Assert.Throws<ArgumentException>(() => Base32.Decode(value));
    }

    [Fact]
    public void Decode_ThrowsArgumentException_ForNonCanonicalEndBits()
    {
        Assert.Throws<ArgumentException>(() => Base32.Decode("ab"));
    }

    [Fact]
    public void Encode_EmptyArray_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, Base32.Encode([]));
    }

    [Fact]
    public void Decode_EmptyString_ReturnsEmptyArray()
    {
        Assert.Empty(Base32.Decode(""));
    }
}