namespace Serenity.Data;

public class RowFieldsProviderTests
{
    /// <summary>
    /// Stub provider used only as a distinguishable identity for Assert.Same checks.
    /// It delegates to FallbackRowFieldsProvider so that rows constructed by other
    /// test classes running in parallel still resolve their fields.
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
    /// RowFieldsProvider.SetLocal is AsyncLocal, but capture the current local
    /// value and reset it so tests are independent, restoring it in a finally block.
    /// </summary>
    private static IRowFieldsProvider? CaptureAndReset()
    {
        return RowFieldsProvider.SetLocal(null);
    }

    private static void Restore(IRowFieldsProvider? local)
    {
        RowFieldsProvider.SetLocal(local);
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
        Assert.Throws<ArgumentNullException>(() =>
            RowFieldsProvider.Resolve<IdNameRow.RowFields>(null));
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
        Assert.Throws<ArgumentNullException>(() => RowFieldsProvider.SetLocalFrom(null));
    }

    [Fact]
    public void SetLocalFrom_WithoutRegistration_ThrowsInvalidOperationException()
    {
        var services = new ServiceCollection().BuildServiceProvider();

        Assert.Throws<InvalidOperationException>(() =>
            RowFieldsProvider.SetLocalFrom(services));
    }
}
