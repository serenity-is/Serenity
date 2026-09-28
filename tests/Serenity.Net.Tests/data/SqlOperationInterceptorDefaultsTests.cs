namespace Serenity.Data;

public class SqlOperationInterceptorDefaultsTests
{
    private class TestInterceptor : ISqlOperationInterceptor
    {
        public InterceptExecuteNonQueryArgs? NonQueryArgs { get; private set; }
        public InterceptExecuteReaderArgs? ReaderArgs { get; private set; }
        public InterceptExecuteScalarArgs? ScalarArgs { get; private set; }

        public OptionalValue<long?> ExecuteNonQuery(InterceptExecuteNonQueryArgs args)
        {
            NonQueryArgs = args;
            return default;
        }

        public OptionalValue<IDataReader> ExecuteReader(InterceptExecuteReaderArgs args)
        {
            ReaderArgs = args;
            return default;
        }

        public OptionalValue<object> ExecuteScalar(InterceptExecuteScalarArgs args)
        {
            ScalarArgs = args;
            return default;
        }
    }

    [Fact]
    public async Task Async_Defaults_Forward_To_Sync_Methods()
    {
        var testInterceptor = new TestInterceptor();
        ISqlOperationInterceptor interceptor = testInterceptor;
        var token = TestContext.Current.CancellationToken;

        var nonQuery = await interceptor.ExecuteNonQueryAsync(
            new InterceptExecuteNonQueryArgs("t", null, ExpectedRows.One, null, false) { CancellationToken = token });
        var reader = await interceptor.ExecuteReaderAsync(
            new InterceptExecuteReaderArgs("t", null, null) { CancellationToken = token });
        var scalar = await interceptor.ExecuteScalarAsync(
            new InterceptExecuteScalarArgs("t", null, null) { CancellationToken = token });

        Assert.False(nonQuery.HasValue);
        Assert.False(reader.HasValue);
        Assert.False(scalar.HasValue);
        Assert.True(testInterceptor.NonQueryArgs!.IsAsync);
        Assert.Equal(token, testInterceptor.NonQueryArgs.CancellationToken);
        Assert.True(testInterceptor.ReaderArgs!.IsAsync);
        Assert.Equal(token, testInterceptor.ReaderArgs.CancellationToken);
        Assert.True(testInterceptor.ScalarArgs!.IsAsync);
        Assert.Equal(token, testInterceptor.ScalarArgs.CancellationToken);
    }
}
