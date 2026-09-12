using MyRow = Serenity.Extensions.Entities.UserPreferenceRow;

namespace Serenity.Extensions.Endpoints;

public class UserPreferenceEndpointTests
{
    private static UserPreferenceEndpoint CreateEndpoint(IRequestContext requestContext)
    {
        var services = new ServiceCollection();
        services.AddSingleton(requestContext);
        return new UserPreferenceEndpoint
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
    public void Update_Delegates_To_Repository()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(_ => new OptionalValue<long?>(1));
        var endpoint = CreateEndpoint(new NullRequestContext().AsGuest("12345"));

        var response = endpoint.Update(new MockUnitOfWork(connection), new UserPreferenceUpdateRequest
        {
            PreferenceType = "t",
            Name = "n",
            Value = "v"
        });

        Assert.NotNull(response);
    }

    [Fact]
    public void Retrieve_Delegates_To_Repository()
    {
        var row = new MyRow { UserPreferenceId = 1, UserId = 12345, PreferenceType = "t", Name = "n", Value = "v" };
        using var connection = new MockDbConnection()
            .InterceptFindRow(_ => new OptionalValue<IRow>(row));
        var endpoint = CreateEndpoint(new NullRequestContext().AsGuest("12345"));

        var response = endpoint.Retrieve(connection, new UserPreferenceRetrieveRequest
        {
            PreferenceType = "t",
            Name = "n"
        });

        Assert.Equal("v", response.Value);
    }
}
