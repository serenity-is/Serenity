using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serenity.Extensions.DependencyInjection;
using Serenity.TestUtils;
using System.Text.Json;

namespace Serenity.Services;

public class ListHandlerSettingsTests
{
    [ReadPermission(SpecialPermissionKeys.Public)]
    private class ListRow : Row<ListRow.RowFields>, IIdRow
    {
        [IdProperty, Identity]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
        }
    }

    private static IRequestContext Context(RequestHandlerSettings? options = null)
    {
        return new DefaultRequestContext(
            new NullBehaviorProvider(),
            new NullTwoLevelCache(),
            NullTextLocalizer.Instance,
            new MockPermissions(_ => true),
            new NullUserAccessor(),
            options);
    }

    private static MockDbConnection Connection() =>
        new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5 }))
            .InterceptExecuteNonQuery(_ => 1);

    private static RequestHandlerSettings Settings(int? maxPageSize = null, int? defaultPageSize = null)
    {
        var settings = new RequestHandlerSettings();
        if (maxPageSize != null)
            settings.List.MaxPageSize = maxPageSize.Value;
        if (defaultPageSize != null)
            settings.List.DefaultPageSize = defaultPageSize.Value;
        return settings;
    }

    [Fact]
    public void Negative_Skip_Throws_ValidationError()
    {
        using var conn = Connection();

        var exception = Assert.Throws<ValidationError>(() =>
            new ListRequestHandler<ListRow>(Context()).List(conn,
                new ListRequest { Skip = -1, ExcludeTotalCount = true }));

        Assert.Equal("ArgumentOutOfRange", exception.ErrorCode);
    }

    [Fact]
    public void Negative_Take_Throws_ValidationError()
    {
        using var conn = Connection();

        var exception = Assert.Throws<ValidationError>(() =>
            new ListRequestHandler<ListRow>(Context()).List(conn,
                new ListRequest { Take = -1, ExcludeTotalCount = true }));

        Assert.Equal("ArgumentOutOfRange", exception.ErrorCode);
    }

    [Fact]
    public void MaxPageSize_Clamps_Take_And_Response_Reports_Applied()
    {
        using var conn = Connection();
        var options = Settings(maxPageSize: 5);

        var response = new ListRequestHandler<ListRow>(Context(options)).List(conn,
            new ListRequest { Take = 100, ExcludeTotalCount = true });

        Assert.Equal(5, response.Take);
    }

    [Fact]
    public void MaxPageSize_Limits_Unspecified_Take()
    {
        using var conn = Connection();
        var options = Settings(maxPageSize: 5);

        var response = new ListRequestHandler<ListRow>(Context(options)).List(conn,
            new ListRequest { ExcludeTotalCount = true });

        Assert.Equal(5, response.Take);
    }

    [Fact]
    public void DefaultPageSize_Applies_When_Take_Unspecified()
    {
        using var conn = Connection();
        var options = Settings(defaultPageSize: 7);

        var response = new ListRequestHandler<ListRow>(Context(options)).List(conn,
            new ListRequest { ExcludeTotalCount = true });

        Assert.Equal(7, response.Take);
    }

    [Fact]
    public void Defaults_Are_Unlimited()
    {
        using var conn = Connection();

        var response = new ListRequestHandler<ListRow>(Context()).List(conn,
            new ListRequest { Take = 100, ExcludeTotalCount = true });

        Assert.Equal(100, response.Take);
    }

    [Fact]
    public void ListSettings_Returns_Default_When_Context_Does_Not_Provide_Settings()
    {
        var handler = new ListRequestHandler<ListRow>(new NullRequestContext().WithPermissions(_ => true));

        Assert.Same(ListHandlerSettings.Default, handler.ListSettings);
    }

    [Fact]
    public void ListSettings_Returns_Settings_From_Context()
    {
        var settings = Settings(maxPageSize: 5);
        var handler = new ListRequestHandler<ListRow>(Context(settings));

        Assert.Same(settings.List, handler.ListSettings);
    }

    private class CustomSettingsHandler(IRequestContext context, ListHandlerSettings settings)
        : ListRequestHandler<ListRow>(context)
    {
        protected override ListHandlerSettings GetListSettings() => settings;
    }

    [Fact]
    public void ListSettings_Can_Be_Overridden_Per_Handler()
    {
        var settings = new ListHandlerSettings { MaxPageSize = 3 };
        var handler = new CustomSettingsHandler(new NullRequestContext().WithPermissions(_ => true), settings);

        Assert.Same(settings, handler.ListSettings);
    }

    [Fact]
    public void Binds_Nested_List_Settings_From_Configuration()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection([new KeyValuePair<string, string?>("RequestHandlers:List:MaxPageSize", "10")])
            .Build();

        var services = new ServiceCollection();
        services.ConfigureSection<RequestHandlerSettings>(config);
        var settings = services.BuildServiceProvider()
            .GetRequiredService<IOptions<RequestHandlerSettings>>().Value;

        Assert.Equal(10, settings.List.MaxPageSize);
    }

    [Fact]
    public void SuppressPagingLimits_Ignores_MaxPageSize()
    {
        using var conn = Connection();
        var settings = Settings(maxPageSize: 5);
        var request = new ListRequest { Take = 100, ExcludeTotalCount = true }.SuppressPagingLimits();

        var response = new ListRequestHandler<ListRow>(Context(settings)).List(conn, request);

        Assert.Equal(100, response.Take);
    }

    [Fact]
    public void SuppressPagingLimits_Ignores_DefaultPageSize()
    {
        using var conn = Connection();
        var settings = Settings(defaultPageSize: 7);
        var request = new ListRequest { ExcludeTotalCount = true }.SuppressPagingLimits();

        var response = new ListRequestHandler<ListRow>(Context(settings)).List(conn, request);

        Assert.Equal(0, response.Take);
    }

    [Fact]
    public void SuppressPagingLimits_False_Applies_Limits()
    {
        using var conn = Connection();
        var settings = Settings(maxPageSize: 5);
        var request = new ListRequest { Take = 100, ExcludeTotalCount = true }.SuppressPagingLimits(false);

        var response = new ListRequestHandler<ListRow>(Context(settings)).List(conn, request);

        Assert.Equal(5, response.Take);
    }

    [Fact]
    public void SuppressPagingLimits_Still_Validates_Negative_Take()
    {
        using var conn = Connection();
        var request = new ListRequest { Take = -1, ExcludeTotalCount = true }.SuppressPagingLimits();

        var exception = Assert.Throws<ValidationError>(() =>
            new ListRequestHandler<ListRow>(Context()).List(conn, request));

        Assert.Equal("ArgumentOutOfRange", exception.ErrorCode);
    }

    [Fact]
    public void SuppressPagingLimits_Is_Not_Bound_From_Json()
    {
        var request = JsonSerializer.Deserialize<ListRequest>(
            "{\"Take\":100,\"pagingLimitsSuppressed\":true,\"PagingLimitsSuppressed\":true}");

        Assert.False(request.IsPagingLimitsSuppressed());
    }
}
