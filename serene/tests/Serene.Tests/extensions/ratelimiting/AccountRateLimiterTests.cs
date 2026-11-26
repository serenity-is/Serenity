using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using System.Threading.RateLimiting;

namespace Serenity.Extensions;

public class AccountRateLimiterTests
{
    private static ServiceProvider CreateServices(Action<AccountRateLimitOptions>? configure = null)
    {
        var services = new ServiceCollection();
        services.AddOptions();
        services.AddLogging();
        services.Configure<AccountRateLimitOptions>(o => configure?.Invoke(o));
        services.AddAccountRateLimiter();
        return services.BuildServiceProvider();
    }

    private static PartitionedRateLimiter<HttpContext> GetLimiter(IServiceProvider services)
    {
        return services.GetRequiredService<IOptions<RateLimiterOptions>>().Value.GlobalLimiter!;
    }

    private static DefaultHttpContext Request(IServiceProvider services, string path,
        string method = "POST", string ip = "10.0.0.1")
    {
        var context = new DefaultHttpContext { RequestServices = services };
        context.Request.Method = method;
        context.Request.Path = path;
        context.Connection.RemoteIpAddress = IPAddress.Parse(ip);
        return context;
    }

    private static int CountAcquired(PartitionedRateLimiter<HttpContext> limiter, Func<HttpContext> request, int attempts)
    {
        var acquired = 0;
        for (var i = 0; i < attempts; i++)
        {
            using var lease = limiter.AttemptAcquire(request());
            if (lease.IsAcquired)
                acquired++;
        }
        return acquired;
    }

    [Fact]
    public void Limits_Post_To_Login_Per_Ip()
    {
        var services = CreateServices(o => o.PermitLimit = 3);
        var limiter = GetLimiter(services);

        Assert.Equal(3, CountAcquired(limiter, () => Request(services, "/Account/Login"), 5));
        // another IP has its own limit
        Assert.Equal(3, CountAcquired(limiter, () => Request(services, "/Account/Login", ip: "10.0.0.2"), 5));
    }

    [Fact]
    public void Each_Path_Has_Its_Own_Limit_And_Matching_Ignores_Case_And_Trailing_Slash()
    {
        var services = CreateServices(o => o.PermitLimit = 2);
        var limiter = GetLimiter(services);

        Assert.Equal(2, CountAcquired(limiter, () => Request(services, "/account/login/"), 4));
        Assert.Equal(2, CountAcquired(limiter, () => Request(services, "/Account/ForgotPassword"), 4));
    }

    [Theory]
    [InlineData("GET", "/Account/Login")]
    [InlineData("POST", "/Account/AccessDenied")]
    [InlineData("POST", "/Services/Northwind/Order/List")]
    public void Does_Not_Limit_Other_Requests(string method, string path)
    {
        var services = CreateServices(o => o.PermitLimit = 1);
        var limiter = GetLimiter(services);

        Assert.Equal(5, CountAcquired(limiter, () => Request(services, path, method), 5));
    }

    [Fact]
    public void Does_Not_Limit_When_Disabled()
    {
        var services = CreateServices(o => { o.PermitLimit = 1; o.Enabled = false; });
        var limiter = GetLimiter(services);

        Assert.Equal(5, CountAcquired(limiter, () => Request(services, "/Account/Login"), 5));
    }

    [Fact]
    public void Uses_Configured_Paths()
    {
        var services = CreateServices(o => { o.PermitLimit = 1; o.Paths = new() { ["/Custom/Login/"] = new() }; });
        var limiter = GetLimiter(services);

        Assert.Equal(1, CountAcquired(limiter, () => Request(services, "/custom/login"), 3));
        Assert.Equal(3, CountAcquired(limiter, () => Request(services, "/Account/Login"), 3));
    }

    [Fact]
    public void Path_Overrides_Global_Limit_And_Zero_Disables_Path()
    {
        var services = CreateServices(o =>
        {
            o.PermitLimit = 2;
            o.Paths["/Account/SignUp"] = new() { PermitLimit = 1 };
            o.Paths["/Account/Elevate"] = new() { PermitLimit = 0 };
        });
        var limiter = GetLimiter(services);

        Assert.Equal(1, CountAcquired(limiter, () => Request(services, "/Account/SignUp"), 3));
        Assert.Equal(3, CountAcquired(limiter, () => Request(services, "/Account/Elevate"), 3));
        Assert.Equal(2, CountAcquired(limiter, () => Request(services, "/Account/Login"), 3));
    }

    [Fact]
    public void Configuration_Is_Merged_Into_Default_Paths()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["AccountRateLimit:Paths:/Account/SignUp:PermitLimit"] = "5",
            ["AccountRateLimit:Paths:/Custom/Action:WindowSeconds"] = "30"
        }).Build();

        var options = new AccountRateLimitOptions();
        configuration.GetSection(AccountRateLimitOptions.SectionKey).Bind(options);

        Assert.Equal(6, options.Paths.Count);
        Assert.Equal(5, options.Paths["/account/signup"].PermitLimit);
        Assert.Null(options.Paths["/Account/Login"].PermitLimit);
        Assert.Equal(30, options.Paths["/Custom/Action"].WindowSeconds);
    }

    [Fact]
    public async Task Rejected_Response_Is_429_With_Service_Error()
    {
        var services = CreateServices(o => o.PermitLimit = 1);
        var options = services.GetRequiredService<IOptions<RateLimiterOptions>>().Value;

        var context = Request(services, "/Account/Login");
        context.Response.Body = new MemoryStream();
        using (options.GlobalLimiter!.AttemptAcquire(context)) { }
        using var lease = options.GlobalLimiter.AttemptAcquire(context);
        Assert.False(lease.IsAcquired);

        await options.OnRejected!(new OnRejectedContext { HttpContext = context, Lease = lease },
            TestContext.Current.CancellationToken);

        Assert.Equal(StatusCodes.Status429TooManyRequests, context.Response.StatusCode);
        context.Response.Body.Position = 0;
        var response = JSON.Parse<ServiceResponse>(new StreamReader(context.Response.Body).ReadToEnd());
        Assert.Equal("TooManyRequests", response.Error?.Code);
        Assert.False(string.IsNullOrEmpty(response.Error?.Message));
    }
}
