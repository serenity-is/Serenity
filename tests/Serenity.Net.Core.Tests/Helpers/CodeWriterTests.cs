using System.Security.Cryptography;

namespace Serenity.Tests.Helpers;

public class Base32Test
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
}