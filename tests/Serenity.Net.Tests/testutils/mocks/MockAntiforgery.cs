using Microsoft.AspNetCore.Antiforgery;
using System.Threading.Tasks;

namespace Serenity.TestUtils;

public class MockAntiforgery() : IAntiforgery
{
    public AntiforgeryTokenSet GetAndStoreTokens(HttpContext httpContext)
    {
        GetTokensCalls++;
        return new AntiforgeryTokenSet("requestToken", "cookieToken", "formFieldName", "headerName");
    }

    public int GetTokensCalls { get; private set; } = 0;

    public AntiforgeryTokenSet GetTokens(HttpContext httpContext)
    {
        GetTokensCalls++;
        return new AntiforgeryTokenSet("requestToken", "cookieToken", "formFieldName", "headerName");
    }

    public int GetAndStoreTokensCalls { get; private set; } = 0;

    public Task<bool> IsRequestValidAsync(HttpContext httpContext)
    {
        IsRequestValidCalls++;
        return Task.FromResult(IsRequestValidValue);
    }

    public int IsRequestValidCalls { get; private set; } = 0;
    public bool IsRequestValidValue { get; set; } = true;

    public void SetCookieTokenAndHeader(HttpContext httpContext)
    {
        SetCookieTokenAndHeaderCalls++;
    }
    
    public int SetCookieTokenAndHeaderCalls { get; private set; } = 0;


    public Task ValidateRequestAsync(HttpContext httpContext)
    {
        ValidateRequestAsyncCalls++;
        if (ValidateRequestAsyncThrows)
            throw new AntiforgeryValidationException("Validation failed in mock.");

        return Task.CompletedTask;
    }

    public bool ValidateRequestAsyncThrows { get; set; } = false;

    public int ValidateRequestAsyncCalls { get; private set; } = 0;
}