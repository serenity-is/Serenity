using Microsoft.AspNetCore.DataProtection;

namespace Serenity.Reporting;

public class HtmlReportCallbackUrlInterceptorTests
{
    private class FakeUserAccessor : IUserAccessor, IImpersonator
    {
        public ClaimsPrincipal? User { get; set; } = new ClaimsPrincipal(new GenericIdentity("current", "Test"));
        public ClaimsPrincipal? Impersonated { get; private set; }
        public int UndoCount { get; private set; }

        public void Impersonate(ClaimsPrincipal user) => Impersonated = user;
        public void UndoImpersonate() => UndoCount++;
    }

    private class FakeClaimCreator : IUserClaimCreator
    {
        public ClaimsPrincipal CreatePrincipal(string username, string authType) =>
            new(new GenericIdentity(username, authType));
    }

    private class FakePermissionService : IPermissionService, ITransientGrantor
    {
        public List<string> Granted { get; } = [];
        public bool AllGranted { get; private set; }
        public int UndoCount { get; private set; }

        public bool HasPermission(string permission) => false;
        public void Grant(params string[] permissions) => Granted.AddRange(permissions);
        public void GrantAll() => AllGranted = true;
        public void UndoGrant() => UndoCount++;
        public bool IsAllGranted() => AllGranted;
        public IEnumerable<string> GetGranted() => Granted;
    }

    private static string CreateToken(IDataProtectionProvider provider, DateTime expiry,
        string username, int grantCount, params string[] grants)
    {
        return provider.CreateProtector(".ReportAuth").ProtectBinary(bw =>
        {
            bw.Write(expiry.ToBinary());
            bw.Write(username);
            bw.Write(grantCount);
            foreach (var g in grants)
                bw.Write(g);
        });
    }

    private static HtmlReportCallbackUrlInterceptor CreateInterceptor(
        IDataProtectionProvider provider, MockHttpContextAccessor accessor,
        FakeUserAccessor? userAccessor = null,
        FakePermissionService? permissionService = null,
        FakeClaimCreator? claimCreator = null,
        ILogger<HtmlReportCallbackUrlBuilder>? logger = null)
    {
        return new HtmlReportCallbackUrlInterceptor(
            logger ?? new MockLogger<HtmlReportCallbackUrlBuilder>(),
            permissionService, userAccessor, claimCreator, accessor, provider);
    }

    [Fact]
    public void InterceptCallback_Throws_For_Null_RenderOptions()
    {
        var interceptor = new HtmlReportCallbackUrlInterceptor(new MockLogger<HtmlReportCallbackUrlBuilder>());
        Assert.Throws<ArgumentNullException>(() => interceptor.InterceptCallback(null!, _ => new()));
    }

    [Fact]
    public void InterceptCallback_Calls_Action_Without_Token()
    {
        var interceptor = CreateInterceptor(new EphemeralDataProtectionProvider(),
            new MockHttpContextAccessor { HttpContext = new DefaultHttpContext() });
        var called = false;

        var result = interceptor.InterceptCallback(new ReportRenderOptions(), _ =>
        {
            called = true;
            return new ReportRenderResult { ViewName = "X" };
        });

        Assert.True(called);
        Assert.Equal("X", result.ViewName);
    }

    [Fact]
    public void InterceptCallback_Impersonates_And_Grants()
    {
        var provider = new EphemeralDataProtectionProvider();
        var userAccessor = new FakeUserAccessor();
        var permissionService = new FakePermissionService();
        var claimCreator = new FakeClaimCreator();
        var httpContext = new DefaultHttpContext();
        var token = CreateToken(provider, DateTime.UtcNow.AddMinutes(5), "other", 2, "p1", "p2");
        httpContext.Request.Headers.Cookie = ".ReportAuth=" + Uri.EscapeDataString(token);
        var interceptor = CreateInterceptor(provider,
            new MockHttpContextAccessor { HttpContext = httpContext }, userAccessor, permissionService, claimCreator);

        interceptor.InterceptCallback(new ReportRenderOptions(), _ => new ReportRenderResult());

        Assert.NotNull(userAccessor.Impersonated);
        Assert.Equal("other", userAccessor.Impersonated!.Identity!.Name);
        Assert.Equal(["p1", "p2"], permissionService.Granted);
        Assert.Equal(1, userAccessor.UndoCount);
        Assert.Equal(1, permissionService.UndoCount);
    }

    [Fact]
    public void InterceptCallback_Grants_All()
    {
        var provider = new EphemeralDataProtectionProvider();
        var permissionService = new FakePermissionService();
        var httpContext = new DefaultHttpContext();
        var token = CreateToken(provider, DateTime.UtcNow.AddMinutes(5), "", -1);
        httpContext.Request.Headers.Cookie = ".ReportAuth=" + Uri.EscapeDataString(token);
        var interceptor = CreateInterceptor(provider,
            new MockHttpContextAccessor { HttpContext = httpContext }, permissionService: permissionService);

        interceptor.InterceptCallback(new ReportRenderOptions(), _ => new ReportRenderResult());

        Assert.True(permissionService.AllGranted);
        Assert.Equal(1, permissionService.UndoCount);
    }

    [Fact]
    public void InterceptCallback_Ignores_Expired_Token()
    {
        var provider = new EphemeralDataProtectionProvider();
        var userAccessor = new FakeUserAccessor();
        var httpContext = new DefaultHttpContext();
        var token = CreateToken(provider, DateTime.UtcNow.AddMinutes(-5), "other", 1, "p1");
        httpContext.Request.Headers.Cookie = ".ReportAuth=" + Uri.EscapeDataString(token);
        var interceptor = CreateInterceptor(provider,
            new MockHttpContextAccessor { HttpContext = httpContext }, userAccessor);

        interceptor.InterceptCallback(new ReportRenderOptions(), _ => new ReportRenderResult());

        Assert.Null(userAccessor.Impersonated);
        Assert.Equal(0, userAccessor.UndoCount);
    }

    [Fact]
    public void InterceptCallback_Logs_Error_For_Invalid_Token()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Cookie = ".ReportAuth=invalidtoken";
        var interceptor = CreateInterceptor(new EphemeralDataProtectionProvider(),
            new MockHttpContextAccessor { HttpContext = httpContext },
            new FakeUserAccessor(), new FakePermissionService(), new FakeClaimCreator(),
            new MockLogger<HtmlReportCallbackUrlBuilder>());

        var called = false;
        interceptor.InterceptCallback(new ReportRenderOptions(), _ =>
        {
            called = true;
            return new ReportRenderResult();
        });

        Assert.True(called);
    }
}
