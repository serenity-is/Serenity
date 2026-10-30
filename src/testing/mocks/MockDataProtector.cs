using Microsoft.AspNetCore.DataProtection;

namespace Serenity.Testing;

public class MockDataProtector(Action<string> protect) : IDataProtector
{
    public IDataProtector CreateProtector(string purpose)
    {
        throw new NotImplementedException();
    }

    public byte[] Protect(byte[] plaintext)
    {
        var data = Encoding.UTF8.GetString(plaintext);
        protect?.Invoke(data);

        return plaintext;
    }

    public byte[] Unprotect(byte[] protectedData)
    {
        return protectedData;
    }
}