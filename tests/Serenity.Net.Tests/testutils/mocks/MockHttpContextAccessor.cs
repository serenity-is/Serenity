namespace Serenity.TestUtils;

public class MockHttpContextAccessor : IHttpContextAccessor
{
    public HttpContext HttpContext { get; set; }
}