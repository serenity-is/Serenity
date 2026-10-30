namespace Serenity.Testing;

public class MockHttpContextAccessor : IHttpContextAccessor
{
    public HttpContext HttpContext { get; set; }
}