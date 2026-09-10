namespace Serenity.Data;

public class RowFieldsProviderTests
{
    /// <summary>
    /// Stub provider used only as a distinguishable identity for Assert.Same checks.
    /// It delegates to FallbackRowFieldsProvider instead of throwing, because while
    /// a test has this installed as the global default provider, other test classes
    /// running in parallel may construct rows, which would otherwise fail with
    /// NotSupportedException (causing intermittent, run-order dependent failures).
    /// </summary>
    private class StubRowFieldsProvider : IRowFieldsProvider
    {
        public RowFieldsBase Resolve(Type fieldsType)
        {
            return FallbackRowFieldsProvider.Instance.Resolve(fieldsType);
        }

        public RowFieldsBase ResolveWithAlias(Type fieldsType, string alias)
        {
            return FallbackRowFieldsProvider.Instance.ResolveWithAlias(fieldsType, alias);
        }
    }

    /// <summary>
    /// RowFieldsProvider members are global statics shared by all tests, so capture
    /// the current state, reset it to known defaults and return it, to be restored
    /// in a finally block.
    /// </summary>
    private static (IRowFieldsProvider? Local, IRowFieldsProvider Default) CaptureAndReset()
    {
        var local = RowFieldsProvider.SetLocal(null);
        var def = RowFieldsProvider.SetDefault(FallbackRowFieldsProvider.Instance);
        return (local, def);
    }

    private static void Restore((IRowFieldsProvider? Local, IRowFieldsProvider Default) state)
    {
        RowFieldsProvider.SetDefault(state.Default);
        RowFieldsProvider.SetLocal(state.Local);
    }

    [Fact]
    public void Current_ReturnsFallbackRowFieldsProvider_ByDefault()
    {
        var state = CaptureAndReset();
        try
        {
            Assert.Same(FallbackRowFieldsProvider.Instance, RowFieldsProvider.Current);
        }
        finally
        {
            Restore(state);
        }
    }

    [Fact]
    public void SetDefault_ReturnsOldDefault_AndSetsNewDefault()
    {
        var state = CaptureAndReset();
        try
        {
            var stub = new StubRowFieldsProvider();

            var old = RowFieldsProvider.SetDefault(stub);

            Assert.Same(FallbackRowFieldsProvider.Instance, old);
            Assert.Same(stub, RowFieldsProvider.Current);
        }
        finally
        {
            Restore(state);
        }
    }

    [Fact]
    public void SetDefault_NullProvider_ThrowsArgumentNullException()
    {
        // throws before assignment, so no state restoration is required
        Assert.Throws<ArgumentNullException>(() => RowFieldsProvider.SetDefault(null!));
    }

    [Fact]
    public void SetLocal_ReturnsOldLocal_AndCurrentReturnsLocal()
    {
        var state = CaptureAndReset();
        try
        {
            var stub = new StubRowFieldsProvider();

            var old = RowFieldsProvider.SetLocal(stub);

            Assert.Null(old);
            Assert.Same(stub, RowFieldsProvider.Current);

            var oldLocal = RowFieldsProvider.SetLocal(null);

            Assert.Same(stub, oldLocal);
            Assert.Same(FallbackRowFieldsProvider.Instance, RowFieldsProvider.Current);
        }
        finally
        {
            Restore(state);
        }
    }

    [Fact]
    public void Resolve_ReturnsFieldsInstance_AndCachesIt()
    {
        var state = CaptureAndReset();
        try
        {
            var fields = RowFieldsProvider.Resolve<IdNameRow.RowFields>();

            Assert.IsType<IdNameRow.RowFields>(fields);
            Assert.Same(fields, RowFieldsProvider.Resolve<IdNameRow.RowFields>());
        }
        finally
        {
            Restore(state);
        }
    }

    [Fact]
    public void Resolve_NullAlias_ThrowsArgumentNullException()
    {
        // throws before the provider is invoked, so no state restoration is required
        Assert.Throws<ArgumentNullException>(() =>
            RowFieldsProvider.Resolve<IdNameRow.RowFields>(null!));
    }

    [Fact]
    public void Resolve_WithAlias_ReplacesAlias_AndCachesPerAlias()
    {
        var state = CaptureAndReset();
        try
        {
            var fields = RowFieldsProvider.Resolve<IdNameRow.RowFields>("x");

            Assert.Equal("x", fields.AliasName);
            Assert.Equal("x", ((IAlias)fields).Name);
            Assert.Equal("x.[ID]", fields.ID.Expression);

            Assert.Same(fields, RowFieldsProvider.Resolve<IdNameRow.RowFields>("x"));

            var unaliased = RowFieldsProvider.Resolve<IdNameRow.RowFields>();
            Assert.NotSame(unaliased, fields);
            Assert.Equal("T0", unaliased.AliasName);
        }
        finally
        {
            Restore(state);
        }
    }

    [Fact]
    public void SetDefaultFrom_SetsDefaultFromServices_AndReturnsOldDefault()
    {
        var state = CaptureAndReset();
        try
        {
            var stub = new StubRowFieldsProvider();
            RowFieldsProvider.SetDefault(stub);

            var services = new ServiceCollection()
                .AddSingleton<IRowFieldsProvider>(FallbackRowFieldsProvider.Instance)
                .BuildServiceProvider();

            var old = RowFieldsProvider.SetDefaultFrom(services);

            Assert.Same(stub, old);
            Assert.Same(FallbackRowFieldsProvider.Instance, RowFieldsProvider.Current);
        }
        finally
        {
            Restore(state);
        }
    }

    [Fact]
    public void SetDefaultFrom_NullServices_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => RowFieldsProvider.SetDefaultFrom(null!));
    }

    [Fact]
    public void SetDefaultFrom_WithoutRegistration_ThrowsInvalidOperationException()
    {
        var services = new ServiceCollection().BuildServiceProvider();

        Assert.Throws<InvalidOperationException>(() =>
            RowFieldsProvider.SetDefaultFrom(services));
    }

    [Fact]
    public void SetLocalFrom_SetsLocalFromServices_AndReturnsOldLocal()
    {
        var state = CaptureAndReset();
        try
        {
            var stub = new StubRowFieldsProvider();
            var oldLocal = RowFieldsProvider.SetLocal(stub);

            var services = new ServiceCollection()
                .AddSingleton<IRowFieldsProvider>(FallbackRowFieldsProvider.Instance)
                .BuildServiceProvider();

            var returned = RowFieldsProvider.SetLocalFrom(services);

            Assert.Same(stub, returned);
            Assert.Same(FallbackRowFieldsProvider.Instance, RowFieldsProvider.Current);
        }
        finally
        {
            Restore(state);
        }
    }

    [Fact]
    public void SetLocalFrom_NullServices_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => RowFieldsProvider.SetLocalFrom(null!));
    }
}
