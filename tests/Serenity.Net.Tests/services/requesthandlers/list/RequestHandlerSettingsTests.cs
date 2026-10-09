using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serenity.Extensions.DependencyInjection;
using Serenity.TestUtils;
using System.Text.Json;

namespace Serenity.Services;

public class RequestHandlerSettingsTests
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

    private static IRequestContext Context(RequestHandlerSettings? settings = null)
    {
        return new DefaultRequestContext(
            new NullBehaviorProvider(),
            new NullTwoLevelCache(),
            NullTextLocalizer.Instance,
            new MockPermissions(_ => true),
            new NullUserAccessor(),
            settings);
    }

    private static MockDbConnection Connection() =>
        new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 5 }))
            .InterceptExecuteNonQuery(_ => 1);

    private static RequestHandlerSettings Settings(int? maxPageSize = null, int? defaultPageSize = null)
    {
        var settings = new RequestHandlerSettings();
        if (maxPageSize != null)
            settings.MaxPageSize = maxPageSize.Value;
        if (defaultPageSize != null)
            settings.DefaultPageSize = defaultPageSize.Value;
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
        var settings = Settings(maxPageSize: 5);

        var response = new ListRequestHandler<ListRow>(Context(settings)).List(conn,
            new ListRequest { Take = 100, ExcludeTotalCount = true });

        Assert.Equal(5, response.Take);
    }

    [Fact]
    public void MaxPageSize_Limits_Unspecified_Take()
    {
        using var conn = Connection();
        var settings = Settings(maxPageSize: 5);

        var response = new ListRequestHandler<ListRow>(Context(settings)).List(conn,
            new ListRequest { ExcludeTotalCount = true });

        Assert.Equal(5, response.Take);
    }

    [Fact]
    public void DefaultPageSize_Applies_When_Take_Unspecified()
    {
        using var conn = Connection();
        var settings = Settings(defaultPageSize: 7);

        var response = new ListRequestHandler<ListRow>(Context(settings)).List(conn,
            new ListRequest { ExcludeTotalCount = true });

        Assert.Equal(7, response.Take);
    }

    [Theory]
    [InlineData(0, 0, false, false)]
    [InlineData(0, 0, true, false)]
    [InlineData(0, 20, false, true)]
    [InlineData(0, 20, true, true)]
    [InlineData(25, 0, false, true)]
    [InlineData(25, 0, true, false)]
    [InlineData(25, 20, true, false)]
    public void ExcludeTotalCountByDefault_And_PagingLimits_Interaction(
        int take, int defaultPageSize, bool excludeByDefault, bool expectCount)
    {
        using var conn = Connection();
        var settings = new RequestHandlerSettings
        {
            DefaultPageSize = defaultPageSize,
            ExcludeTotalCountByDefault = excludeByDefault
        };

        var response = new ListRequestHandler<ListRow>(Context(settings)).List(conn,
            new ListRequest { Take = take });

        Assert.Equal(take == 0 ? defaultPageSize : take, response.Take);

        var commandText = conn.ExecuteReaderCalls.Single().CommandText;
        Assert.Equal(expectCount, commandText.Contains("count(*)", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void IncludeMore_Is_Not_Overridden_When_PagingLimits_Apply_Take()
    {
        using var conn = Connection();
        var settings = new RequestHandlerSettings { DefaultPageSize = 20 };

        var response = new ListRequestHandler<ListRow>(Context(settings)).List(conn,
            new ListRequest { IncludeMore = true });

        Assert.Equal(20, response.Take);
        Assert.False(response.More);

        var commandText = conn.ExecuteReaderCalls.Single().CommandText;
        Assert.DoesNotContain("count(*)", commandText, StringComparison.OrdinalIgnoreCase);
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
    public void HandlerSettings_Returns_Default_When_Context_Does_Not_Provide_Settings()
    {
        var handler = new ListRequestHandler<ListRow>(new NullRequestContext().WithPermissions(_ => true));

        Assert.Same(RequestHandlerSettings.Default, handler.HandlerSettings);
    }

    [Fact]
    public void HandlerSettings_Returns_Settings_From_Context()
    {
        var settings = Settings(maxPageSize: 5);
        var handler = new ListRequestHandler<ListRow>(Context(settings));

        Assert.Same(settings, handler.HandlerSettings);
    }

    private class CustomSettingsHandler(IRequestContext context, RequestHandlerSettings settings)
        : ListRequestHandler<ListRow>(context)
    {
        protected override RequestHandlerSettings GetHandlerSettings() => settings;
    }

    [Fact]
    public void HandlerSettings_Can_Be_Overridden_Per_Handler()
    {
        var settings = new RequestHandlerSettings { MaxPageSize = 3 };
        var handler = new CustomSettingsHandler(new NullRequestContext().WithPermissions(_ => true), settings);

        Assert.Same(settings, handler.HandlerSettings);
    }

    [Fact]
    public void With_Creates_Modified_Copy_Without_Changing_Original()
    {
        var settings = Settings(maxPageSize: 5, defaultPageSize: 10);

        var copy = settings.With(x => x.MaxPageSize = 0);

        Assert.Equal(0, copy.MaxPageSize);
        Assert.Equal(10, copy.DefaultPageSize);
        Assert.Equal(5, settings.MaxPageSize);
        Assert.Equal(10, settings.DefaultPageSize);
        Assert.NotSame(settings, copy);
    }

    [Fact]
    public void Binds_Settings_From_Configuration()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection([new KeyValuePair<string, string?>("RequestHandlerSettings:MaxPageSize", "10")])
            .Build();

        var services = new ServiceCollection();
        services.ConfigureSection<RequestHandlerSettings>(config);
        var settings = services.BuildServiceProvider()
            .GetRequiredService<IOptions<RequestHandlerSettings>>().Value;

        Assert.Equal(10, settings.MaxPageSize);
    }

    [Fact]
    public void ExcludeTotalCountByDefault_Defaults_To_False()
    {
        Assert.False(RequestHandlerSettings.Default.ExcludeTotalCountByDefault);
        Assert.False(new RequestHandlerSettings().ExcludeTotalCountByDefault);
    }

    [Fact]
    public void Binds_ExcludeTotalCountByDefault_From_Configuration()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection([new KeyValuePair<string, string?>("RequestHandlerSettings:ExcludeTotalCountByDefault", "true")])
            .Build();

        var services = new ServiceCollection();
        services.ConfigureSection<RequestHandlerSettings>(config);
        var settings = services.BuildServiceProvider()
            .GetRequiredService<IOptions<RequestHandlerSettings>>().Value;

        Assert.True(settings.ExcludeTotalCountByDefault);
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
