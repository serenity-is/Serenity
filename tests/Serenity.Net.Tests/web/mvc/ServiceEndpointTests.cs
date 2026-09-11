using Microsoft.AspNetCore.Mvc.Controllers;

namespace Serenity.Services;

public class ServiceEndpointTests
{
    [ConnectionKey("Test")]
    private class TestEndpoint : ServiceEndpoint
    {
        public void Dummy()
        {
        }
    }

    private static TestEndpoint CreateEndpoint()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ISqlConnections>(new MockSqlConnections { OnNewByKey = _ => new MockDbConnection() });

        var endpoint = new TestEndpoint
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = services.BuildServiceProvider()
                }
            }
        };

        return endpoint;
    }

    private static ActionExecutingContext CreateExecutingContext(TestEndpoint endpoint)
    {
        var actionDescriptor = new ControllerActionDescriptor
        {
            MethodInfo = typeof(TestEndpoint).GetMethod(nameof(TestEndpoint.Dummy)),
            Parameters =
            [
                new ParameterDescriptor
                {
                    Name = "uow",
                    ParameterType = typeof(IUnitOfWork)
                }
            ]
        };

        var actionContext = new ActionContext(endpoint.HttpContext, new RouteData(), actionDescriptor);

        return new ActionExecutingContext(actionContext, [], new Dictionary<string, object?>(), endpoint);
    }

    [Fact]
    public async Task OnActionExecutionAsync_Commits_Action()
    {
        var endpoint = CreateEndpoint();
        var executingContext = CreateExecutingContext(endpoint);
        var executedContext = new ActionExecutedContext(executingContext, [], endpoint);
        int commitCalls = 0;

        await endpoint.OnActionExecutionAsync(executingContext, () =>
        {
            if (executingContext.ActionArguments["uow"] is UnitOfWork uow)
                uow.OnCommit += () => commitCalls++;
            return Task.FromResult(executedContext);
        });

        Assert.Equal(1, commitCalls);
        Assert.NotNull(executedContext.Result);
    }

    [Fact]
    public async Task OnActionExecutionAsync_RollsBack_When_Action_Throws()
    {
        var endpoint = CreateEndpoint();
        var executingContext = CreateExecutingContext(endpoint);
        var executedContext = new ActionExecutedContext(executingContext, [], endpoint)
        {
            Exception = new ValidationError("test error")
        };
        int rollbackCalls = 0;

        await endpoint.OnActionExecutionAsync(executingContext, () =>
        {
            if (executingContext.ActionArguments["uow"] is UnitOfWork uow)
                uow.OnRollback += () => rollbackCalls++;
            return Task.FromResult(executedContext);
        });

        Assert.Equal(1, rollbackCalls);
    }

    [Fact]
    public async Task DisposeAsync_Disposes_UnitOfWork()
    {
        var endpoint = CreateEndpoint();
        var executingContext = CreateExecutingContext(endpoint);
        endpoint.OnActionExecuting(executingContext);
        int rollbackCalls = 0;

        if (executingContext.ActionArguments["uow"] is UnitOfWork uow)
            uow.OnRollback += () => rollbackCalls++;

        await endpoint.DisposeAsync();

        Assert.Equal(1, rollbackCalls);
    }

    [Fact]
    public async Task DisposeAsync_DoesNot_Dispose_Again()
    {
        var endpoint = CreateEndpoint();
        var executingContext = CreateExecutingContext(endpoint);
        endpoint.OnActionExecuting(executingContext);
        int rollbackCalls = 0;

        if (executingContext.ActionArguments["uow"] is UnitOfWork uow)
            uow.OnRollback += () => rollbackCalls++;

        await endpoint.DisposeAsync();
        await endpoint.DisposeAsync();

        Assert.Equal(1, rollbackCalls);
    }
}
