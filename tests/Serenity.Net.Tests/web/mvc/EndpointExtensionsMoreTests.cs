using Microsoft.Extensions.Logging;

namespace Serenity.Services;

public class EndpointExtensionsMoreTests
{
    private class TestController : ControllerBase
    {
    }

    private static TestController CreateController(Action<IServiceCollection>? configure = null)
    {
        var services = new ServiceCollection();
        configure?.Invoke(services);
        return new TestController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = services.BuildServiceProvider()
                }
            }
        };
    }

    [Fact]
    public void ConvertToResponse_From_HttpContext_Works_When_RequestServices_Is_Null()
    {
        var httpContext = new DefaultHttpContext();
        var exception = new ValidationError("message");

        var response = exception.ConvertToResponse<ServiceResponse>(httpContext);

        Assert.Equal("message", response.Error!.Message);
    }

    [Fact]
    public void ConvertToResponse_From_HttpContext_Shows_Details_In_Development()
    {
        var env = new MockHostEnvironment { EnvironmentName = "Development" };
        var controller = CreateController(services =>
        {
            services.AddSingleton<ILogger<ServiceEndpoint>>(new MockLogger<ServiceEndpoint>());
            services.AddSingleton<ITextLocalizer>(NullTextLocalizer.Instance);
            services.AddSingleton<IWebHostEnvironment>(env);
        });

        var response = new ValidationError("message")
            .ConvertToResponse<ServiceResponse>(controller.HttpContext);

        Assert.Equal("message", response.Error!.Message);
        Assert.NotNull(response.Error.Details);
    }

    [Fact]
    public void ConvertToResponse_From_HttpContext_Hides_Sensitive_Details_In_Production()
    {
        var env = new MockHostEnvironment { EnvironmentName = "Production" };
        var controller = CreateController(services =>
        {
            services.AddSingleton<ITextLocalizer>(new MockTextLocalizer());
            services.AddSingleton<IWebHostEnvironment>(env);
        });

        var response = new ValidationError("sensitive")
        {
            IsSensitiveMessage = true
        }.ConvertToResponse<ServiceResponse>(controller.HttpContext);

        Assert.Equal("Services.GenericErrorMessage", response.Error!.Message);
    }

    [Fact]
    public void ConvertToResponse_Sets_ErrorId_From_Exception_Data()
    {
        var exception = new InvalidOperationException("boom");
        exception.Data[nameof(ServiceError.ErrorId)] = 42;

        var response = exception.ConvertToResponse<ServiceResponse>(new MockLogger(), NullTextLocalizer.Instance, true);

        Assert.Equal("Exception", response.Error!.Code);
        Assert.Equal("42", response.Error.ErrorId);
    }

    [Fact]
    public void ConvertToResponse_Sets_ValidationError_Arguments()
    {
        var exception = new ValidationError("code", "args", "message");

        var response = exception.ConvertToResponse<ServiceResponse>(new MockLogger(), NullTextLocalizer.Instance, false);

        Assert.Equal("args", response.Error!.Arguments);
    }

    [Fact]
    public void ExecuteMethod_Returns_Handler_Result()
    {
        var controller = CreateController();
        var result = controller.ExecuteMethod<ServiceResponse>(() => new ServiceResponse());

        Assert.NotNull(result.Data);
        Assert.Null(result.Data.Error);
    }

    [Fact]
    public void ExecuteMethod_Converts_ValidationError_To_400()
    {
        var controller = CreateController();
        var result = controller.ExecuteMethod<ServiceResponse>(() => throw new ValidationError("test error"));

        Assert.Equal(400, controller.HttpContext.Response.StatusCode);
        Assert.Equal("test error", result.Data!.Error!.Message);
    }

    [Fact]
    public void ExecuteMethod_Converts_Other_Exceptions_To_500()
    {
        var controller = CreateController();
        controller.HttpContext.Response.Body = new System.IO.MemoryStream();

        var result = controller.ExecuteMethod<ServiceResponse>(() => throw new InvalidOperationException("boom"));

        Assert.Equal(500, controller.HttpContext.Response.StatusCode);
        Assert.Equal("Exception", result.Data!.Error!.Code);
    }

    [Fact]
    public async Task ExecuteMethodAsync_Returns_Handler_Result()
    {
        var controller = CreateController();
        var result = await controller.ExecuteMethodAsync<ServiceResponse>(() => Task.FromResult(new ServiceResponse()));

        Assert.NotNull(result.Data);
        Assert.Null(result.Data.Error);
    }

    [Fact]
    public async Task ExecuteMethodAsync_Converts_Exception_To_Response()
    {
        var controller = CreateController();
        var result = await controller.ExecuteMethodAsync<ServiceResponse>(
            () => throw new ValidationError("async error"));

        Assert.Equal(400, controller.HttpContext.Response.StatusCode);
        Assert.Equal("async error", result.Data!.Error!.Message);
    }

    [Fact]
    public void UseConnection_Passes_Connection_To_Handler_And_Disposes_It()
    {
        var connection = new MockDbConnection();
        var controller = CreateController(services =>
        {
            services.AddSingleton<ISqlConnections>(new MockSqlConnections { OnNewByKey = _ => connection });
        });

        IDbConnection? passed = null;
        var result = controller.UseConnection<ServiceResponse>("Test", c =>
        {
            passed = c;
            return new ServiceResponse();
        });

        Assert.Same(connection, passed);
        Assert.NotNull(result.Data);
        Assert.Equal(ConnectionState.Closed, connection.State);
    }

    [Fact]
    public void UseConnection_Converts_Exception_To_Response()
    {
        var connection = new MockDbConnection();
        var controller = CreateController(services =>
        {
            services.AddSingleton<ISqlConnections>(new MockSqlConnections { OnNewByKey = _ => connection });
        });
        controller.HttpContext.Response.Body = new System.IO.MemoryStream();

        var result = controller.UseConnection<ServiceResponse>("Test", _ => throw new ValidationError("conn error"));

        Assert.Equal(400, controller.HttpContext.Response.StatusCode);
        Assert.Equal("conn error", result.Data!.Error!.Message);
    }

    [Fact]
    public void InTransaction_Commits_And_Returns_Response()
    {
        var connection = new MockDbConnection();
        var controller = CreateController(services =>
        {
            services.AddSingleton<ISqlConnections>(new MockSqlConnections { OnNewByKey = _ => connection });
        });

        int commitCalls = 0;
        var result = controller.InTransaction<ServiceResponse>("Test", uow =>
        {
            uow.OnCommit += () => commitCalls++;
            return new ServiceResponse();
        });

        Assert.Equal(1, commitCalls);
        Assert.NotNull(result.Data);
        Assert.Null(result.Data.Error);
    }

    [Fact]
    public void InTransaction_Converts_Exception_To_Response()
    {
        var connection = new MockDbConnection();
        var controller = CreateController(services =>
        {
            services.AddSingleton<ISqlConnections>(new MockSqlConnections { OnNewByKey = _ => connection });
        });
        controller.HttpContext.Response.Body = new System.IO.MemoryStream();

        int commitCalls = 0;
        var result = controller.InTransaction<ServiceResponse>("Test", uow =>
        {
            uow.OnCommit += () => commitCalls++;
            throw new ValidationError("tx error");
        });

        Assert.Equal(0, commitCalls);
        Assert.Equal(400, controller.HttpContext.Response.StatusCode);
        Assert.Equal("tx error", result.Data!.Error!.Message);
    }
}
