namespace Serenity.Services;

public class EndpointExtensionTests
{
    [Fact]
    public void ConvertToResponse_ShowsDetails_WhenShowDetailsIsTrue()
    {
        var mockExceptionLogger = new MockLogger();
        var exception = new ValidationError("Not Sensitive Error");

        var response = exception.ConvertToResponse<ServiceResponse>(mockExceptionLogger, NullTextLocalizer.Instance, showDetails: true);

        Assert.Equal("Not Sensitive Error", response.Error.Message);
    }

    [Fact]
    public void ConvertToResponse_HidesDetails_WhenShowDetailsIsFalse_AndErrorIsSensitive()
    {
        var mockExceptionLogger = new MockLogger();
        var exception = new ValidationError("Sensitive Error")
        {
            IsSensitiveMessage = true
        };

        var response = exception.ConvertToResponse<ServiceResponse>(mockExceptionLogger, new MockTextLocalizer(), showDetails: false);
        Assert.Equal("Services.GenericErrorMessage", response.Error.Message);
    }

    [Fact]
    public void ConvertToResponse_ShowsDetails_WhenShowDetailsIsFalse_AndErrorIsNotSensitive()
    {
        var mockExceptionLogger = new MockLogger();
        var exception = new ValidationError("Not Sensitive Error")
        {
            IsSensitiveMessage = false
        };

        var response = exception.ConvertToResponse<ServiceResponse>(mockExceptionLogger, NullTextLocalizer.Instance, showDetails: false);
        Assert.Equal("Not Sensitive Error", response.Error.Message);
    }

    [Fact]
    public void ConvertToResponse_HidesDetails_AndUsesConstantErrorMessage_WhenShowDetailsIsFalse_AndErrorIsSensitive_AndLocalizerIsNull()
    {
        var mockExceptionLogger = new MockLogger();
        var exception = new ValidationError("Sensitive Error")
        {
            IsSensitiveMessage = true
        };

        var response = exception.ConvertToResponse<ServiceResponse>(mockExceptionLogger, null, showDetails: false);
        Assert.Equal("An error occurred while processing your request.", response.Error.Message);
    }

    [Fact]
    public async Task InTransactionAsync_Commits_And_ReturnsResponse()
    {
        var connection = new MockDbConnection();
        var services = new ServiceCollection();
        services.AddSingleton<ISqlConnections>(new MockSqlConnections { OnNewByKey = _ => connection });
        var controller = new TestController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = services.BuildServiceProvider()
                }
            }
        };

        int commitCalls = 0;
        var result = await controller.InTransactionAsync<ServiceResponse>("Test",
            (uow, ct) =>
            {
                uow.OnCommit += () => commitCalls++;
                return Task.FromResult(new ServiceResponse());
            }, TestContext.Current.CancellationToken);

        Assert.Equal(1, commitCalls);
        Assert.NotNull(result.Data);
        Assert.Null(result.Data.Error);
    }

    [Fact]
    public async Task InTransactionAsync_ConvertsException_ToResponse_And_DoesNotCommit()
    {
        var connection = new MockDbConnection();
        var services = new ServiceCollection();
        services.AddSingleton<ISqlConnections>(new MockSqlConnections { OnNewByKey = _ => connection });
        var controller = new TestController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = services.BuildServiceProvider()
                }
            }
        };

        int commitCalls = 0;
        var result = await controller.InTransactionAsync<ServiceResponse>("Test",
            (uow, ct) =>
            {
                uow.OnCommit += () => commitCalls++;
                throw new ValidationError("test error");
            }, TestContext.Current.CancellationToken);

        Assert.Equal(0, commitCalls);
        Assert.NotNull(result.Data.Error);
        Assert.Equal("test error", result.Data.Error.Message);
    }

    private class TestController : ControllerBase
    {
    }
}