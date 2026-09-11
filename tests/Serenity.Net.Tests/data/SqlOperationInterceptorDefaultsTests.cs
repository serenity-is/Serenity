namespace Serenity.Data;

public class SqlOperationInterceptorDefaultsTests
{
    private class TestInterceptor : ISqlOperationInterceptor
    {
        public OptionalValue<long?> ExecuteNonQuery(string commandText, IDictionary<string, object?>? parameters,
            ExpectedRows expectedRows, IQueryWithParams? query, bool getNewId) => default;

        public OptionalValue<IDataReader> ExecuteReader(string commandText, IDictionary<string, object?>? parameters,
            SqlQuery? query) => default;

        public OptionalValue<object> ExecuteScalar(string commandText, IDictionary<string, object?>? parameters,
            SqlQuery? query) => default;
    }

    [Fact]
    public async Task Async_Defaults_Forward_To_Sync_Methods()
    {
        ISqlOperationInterceptor interceptor = new TestInterceptor();
        var token = TestContext.Current.CancellationToken;

        var nonQuery = await interceptor.ExecuteNonQueryAsync("t", null, ExpectedRows.One, null, false, token);
        var reader = await interceptor.ExecuteReaderAsync("t", null, null, token);
        var scalar = await interceptor.ExecuteScalarAsync("t", null, null, token);

        Assert.False(nonQuery.HasValue);
        Assert.False(reader.HasValue);
        Assert.False(scalar.HasValue);
    }
}
