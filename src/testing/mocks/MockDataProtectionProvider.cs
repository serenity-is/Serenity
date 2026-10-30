using Microsoft.AspNetCore.DataProtection;

namespace Serenity.Testing;

public class MockDataProtectionProvider(Action<string> protect) : IDataProtectionProvider
{
    public IDataProtector CreateProtector(string purpose)
    {
        return new MockDataProtector(protect);
    }
}