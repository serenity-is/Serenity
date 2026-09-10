namespace Serenity.Data;

public class DefaultRowFieldsProviderTests
{
    private interface ITestDependency
    {
    }

    private class TestDependency : ITestDependency
    {
    }

    [ConnectionKey("TestKey")]
    private class ConnectionKeyRow : Row<ConnectionKeyRow.RowFields>
    {
        [IdProperty, Identity]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
#pragma warning restore CS0649
        }
    }

    private class DependencyRow : Row<DependencyRow.RowFields>
    {
        [IdProperty, Identity]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        public class RowFields(ITestDependency dependency) : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
#pragma warning restore CS0649

            public ITestDependency? Dependency { get; } = dependency;
        }
    }

    private class MockConnectionString(string connectionKey, ISqlDialect dialect) : IConnectionString
    {
        public ISqlDialect Dialect { get; } = dialect;

        public string ConnectionKey { get; } = connectionKey;

        public string ConnectionString { get; } = "TestConnectionString";

        public string ProviderName { get; } = "TestProvider";
    }

    private class MockConnectionStrings(params IConnectionString[] connections) : IConnectionStrings
    {
        public IConnectionString? TryGetConnectionString(string connectionKey)
        {
            return connections.FirstOrDefault(x => x.ConnectionKey == connectionKey);
        }

        public IEnumerable<IConnectionString> ListConnectionStrings()
        {
            return connections;
        }
    }

    private static DefaultRowFieldsProvider CreateProvider(Action<ServiceCollection>? setup = null)
    {
        var services = new ServiceCollection();
        setup?.Invoke(services);
        return new DefaultRowFieldsProvider(services.BuildServiceProvider());
    }

    [Fact]
    public void Constructor_NullServiceProvider_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DefaultRowFieldsProvider(null!));
    }

    [Fact]
    public void Resolve_ReturnsFieldsInstance()
    {
        var provider = CreateProvider();

        var fields = provider.Resolve(typeof(IdNameRow.RowFields));

        Assert.IsType<IdNameRow.RowFields>(fields);
    }

    [Fact]
    public void Resolve_CachesInstancePerType()
    {
        var provider = CreateProvider();

        var first = provider.Resolve(typeof(IdNameRow.RowFields));
        var second = provider.Resolve(typeof(IdNameRow.RowFields));

        Assert.Same(first, second);
    }

    [Fact]
    public void Resolve_InitializesFields()
    {
        var provider = CreateProvider();

        var fields = (IdNameRow.RowFields)provider.Resolve(typeof(IdNameRow.RowFields));

        // TableName property throws if the fields are not initialized
        Assert.Equal("IdName", fields.TableName);
    }

    [Fact]
    public void ResolveWithAlias_NullAlias_ThrowsArgumentNullException()
    {
        var provider = CreateProvider();

        Assert.Throws<ArgumentNullException>(() =>
            provider.ResolveWithAlias(typeof(IdNameRow.RowFields), null!));
    }

    [Fact]
    public void ResolveWithAlias_EmptyAlias_ThrowsArgumentNullException()
    {
        var provider = CreateProvider();

        Assert.Throws<ArgumentNullException>(() =>
            provider.ResolveWithAlias(typeof(IdNameRow.RowFields), string.Empty));
    }

    [Fact]
    public void ResolveWithAlias_SetsAlias()
    {
        var provider = CreateProvider();

        var fields = provider.ResolveWithAlias(typeof(IdNameRow.RowFields), "x");

        Assert.Equal("x", fields.AliasName);
        Assert.Equal("x", ((IAlias)fields).Name);
    }

    [Fact]
    public void ResolveWithAlias_CachesPerTypeAndAlias()
    {
        var provider = CreateProvider();

        var first = provider.ResolveWithAlias(typeof(IdNameRow.RowFields), "x");
        var second = provider.ResolveWithAlias(typeof(IdNameRow.RowFields), "x");
        var other = provider.ResolveWithAlias(typeof(IdNameRow.RowFields), "y");

        Assert.Same(first, second);
        Assert.NotSame(first, other);
        Assert.Equal("y", other.AliasName);
    }

    [Fact]
    public void ResolveWithAlias_ReplacesFieldExpressions()
    {
        var provider = CreateProvider();

        var fields = (IdNameRow.RowFields)provider.ResolveWithAlias(typeof(IdNameRow.RowFields), "x");

        Assert.Equal("x.[ID]", fields.ID.Expression);
    }

    [Fact]
    public void Resolve_InjectsConstructorDependencyFromServices()
    {
        var dependency = new TestDependency();
        var provider = CreateProvider(services =>
            services.AddSingleton<ITestDependency>(dependency));

        var fields = Assert.IsType<DependencyRow.RowFields>(
            provider.Resolve(typeof(DependencyRow.RowFields)));

        Assert.Same(dependency, fields.Dependency);
    }

    [Fact]
    public void Resolve_UsesDialectFromConnectionStrings()
    {
        var provider = CreateProvider(services =>
            services.AddSingleton<IConnectionStrings>(new MockConnectionStrings(
                new MockConnectionString("TestKey", MySqlDialect.Instance))));

        var fields = (ConnectionKeyRow.RowFields)provider.Resolve(typeof(ConnectionKeyRow.RowFields));

        Assert.Same(MySqlDialect.Instance, fields.Dialect);
    }

    [Fact]
    public void Resolve_FallsBackToDefaultDialect_WhenConnectionStringsNotRegistered()
    {
        var provider = CreateProvider();
        var expected = SqlSettings.DefaultDialect;

        var fields = (ConnectionKeyRow.RowFields)provider.Resolve(typeof(ConnectionKeyRow.RowFields));

        Assert.Same(expected, fields.Dialect);
    }
}
