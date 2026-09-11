using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Serenity.Services;

public class ServiceEndpointMoreTests
{
    [ConnectionKey("Test")]
    private class TestEndpoint : ServiceEndpoint
    {
        public void UowAction(IUnitOfWork uow)
        {
        }

        public void ConnAction(IDbConnection connection)
        {
        }

        public IRequestContext ContextPublic => Context;
        public bool HasContextPublic => HasRequestContext;
        public ITwoLevelCache CachePublic => Cache;
        public ITextLocalizer LocalizerPublic => Localizer;
        public IPermissionService PermissionsPublic => Permissions;
        public void SetContext(IRequestContext context) => Context = context;
        public void ExecutedSync(ActionExecutedContext context) => OnActionExecuted(context);
    }

    private class NoKeyEndpoint : ServiceEndpoint
    {
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0060 // Remove unused parameter
        public void UowAction(IUnitOfWork uow)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1822 // Mark members as static
        {
        }
    }

    [ConnectionKey("Test")]
    [TransactionSettings(IsolationLevel.ReadCommitted, DeferStart = true)]
    private class SettingsEndpoint : ServiceEndpoint
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public void UowAction(IUnitOfWork uow)
#pragma warning restore IDE0060 // Remove unused parameter
        {
        }
    }

    private class NonActionResult : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context) => Task.CompletedTask;
    }

    private static TestEndpoint CreateEndpoint(string methodName, bool withRequestContext = true)
    {
        var services = new ServiceCollection();
        services.AddSingleton<ISqlConnections>(new MockSqlConnections { OnNewByKey = _ => new MockDbConnection() });
        if (withRequestContext)
            services.AddSingleton<IRequestContext>(new NullRequestContext());

        return new TestEndpoint
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { RequestServices = services.BuildServiceProvider() }
            }
        };
    }

    private static ActionExecutingContext CreateExecutingContext(ServiceEndpoint endpoint, string methodName)
    {
        var method = endpoint.GetType().GetMethod(methodName)!;
        var actionDescriptor = new ControllerActionDescriptor
        {
            MethodInfo = method,
            Parameters =
            [
                new ParameterDescriptor
                {
                    Name = method.GetParameters()[0].Name,
                    ParameterType = method.GetParameters()[0].ParameterType
                }
            ]
        };
        var actionContext = new ActionContext(endpoint.HttpContext, new RouteData(), actionDescriptor);
        return new ActionExecutingContext(actionContext, [], new Dictionary<string, object?>(), endpoint);
    }

    [Fact]
    public void OnActionExecuting_With_UnitOfWork_Parameter_Creates_UnitOfWork()
    {
        var endpoint = CreateEndpoint(nameof(TestEndpoint.UowAction));
        var context = CreateExecutingContext(endpoint, nameof(TestEndpoint.UowAction));

        endpoint.OnActionExecuting(context);

        Assert.IsType<UnitOfWork>(context.ActionArguments["uow"]);
    }

    [Fact]
    public void OnActionExecuting_With_Connection_Parameter_Sets_Argument()
    {
        var endpoint = CreateEndpoint(nameof(TestEndpoint.ConnAction));
        var context = CreateExecutingContext(endpoint, nameof(TestEndpoint.ConnAction));

        endpoint.OnActionExecuting(context);

        Assert.NotNull(context.ActionArguments["connection"]);
    }

    [Fact]
    public void OnActionExecuting_Throws_Without_ConnectionKey()
    {
        var endpoint = new NoKeyEndpoint
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = new ServiceCollection()
                        .AddSingleton<ISqlConnections>(new MockSqlConnections { OnNewByKey = _ => new MockDbConnection() })
                        .BuildServiceProvider()
                }
            }
        };
        var context = CreateExecutingContext(endpoint, nameof(NoKeyEndpoint.UowAction));

        Assert.Throws<ArgumentNullException>(() => endpoint.OnActionExecuting(context));
    }

    [Fact]
    public void OnActionExecuting_Uses_TransactionSettings_Attribute()
    {
        var services = new ServiceCollection()
            .AddSingleton<ISqlConnections>(new MockSqlConnections { OnNewByKey = _ => new MockDbConnection() })
            .BuildServiceProvider();
        var endpoint = new SettingsEndpoint
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { RequestServices = services }
            }
        };
        var context = CreateExecutingContext(endpoint, nameof(SettingsEndpoint.UowAction));

        endpoint.OnActionExecuting(context);

        Assert.IsType<UnitOfWork>(context.ActionArguments["uow"]);
    }

    [Fact]
    public void OnActionExecuted_Commits_And_Wraps_Result()
    {
        var endpoint = CreateEndpoint(nameof(TestEndpoint.UowAction));
        var executing = CreateExecutingContext(endpoint, nameof(TestEndpoint.UowAction));
        endpoint.OnActionExecuting(executing);
        int commits = 0;
        ((UnitOfWork)executing.ActionArguments["uow"]!).OnCommit += () => commits++;

        var executedContext = new ActionExecutedContext(
            executing, [], endpoint) { Result = new NonActionResult() };
        endpoint.ExecutedSync(executedContext);

        Assert.Equal(1, commits);
        Assert.IsType<Result<object?>>(executedContext.Result);
    }

    [Fact]
    public void OnActionExecuted_Disposes_On_Exception()
    {
        var endpoint = CreateEndpoint(nameof(TestEndpoint.UowAction));
        var executing = CreateExecutingContext(endpoint, nameof(TestEndpoint.UowAction));
        endpoint.OnActionExecuting(executing);
        int rollbacks = 0;
        ((UnitOfWork)executing.ActionArguments["uow"]!).OnRollback += () => rollbacks++;

        var executedContext = new ActionExecutedContext(
            executing, [], endpoint) { Exception = new ValidationError("err") };
        endpoint.ExecutedSync(executedContext);

        Assert.Equal(1, rollbacks);
    }

    [Fact]
    public void Context_Properties_Return_Request_Context_Services()
    {
        var endpoint = CreateEndpoint(nameof(TestEndpoint.UowAction));
        var requestContext = endpoint.ContextPublic;

        Assert.NotNull(requestContext);
        Assert.Same(requestContext.Cache, endpoint.CachePublic);
        Assert.Same(requestContext.Localizer, endpoint.LocalizerPublic);
        Assert.Same(requestContext.Permissions, endpoint.PermissionsPublic);
        Assert.True(endpoint.HasContextPublic);
    }

    [Fact]
    public void Context_Throws_When_Request_Context_Not_Registered()
    {
        var endpoint = CreateEndpoint(nameof(TestEndpoint.UowAction), withRequestContext: false);
        Assert.Throws<InvalidOperationException>(() => endpoint.ContextPublic);
    }

    [Fact]
    public void Context_Setter_Throws_For_Null()
    {
        var endpoint = CreateEndpoint(nameof(TestEndpoint.UowAction));
        Assert.Throws<ArgumentNullException>(() => endpoint.SetContext(null!));
    }

    [Fact]
    public void Dispose_Releases_Resources()
    {
        var endpoint = CreateEndpoint(nameof(TestEndpoint.UowAction));
        var context = CreateExecutingContext(endpoint, nameof(TestEndpoint.UowAction));
        endpoint.OnActionExecuting(context);

        endpoint.Dispose();
        endpoint.Dispose();
    }
}
