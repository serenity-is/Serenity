using System.Collections;

namespace Serenity.Data;

public class RowOperationInterceptorTests
{
    private sealed class TestInterceptor : IRowOperationInterceptor
    {
        public OptionalValue<IRow> FindRow(FindRowArgs args)
            => new(new IdNameRow());

        public OptionalValue<IList> ListRows(ListRowsArgs args)
            => new(new List<IRow>());

        public OptionalValue<long?> ManipulateRow(Type rowType, OptionalValue<object?> id,
            IRow? row, ExpectedRows expectedRows, bool getNewId)
            => new(1L);
    }

    [Fact]
    public async Task Async_Methods_Forward_To_Sync()
    {
        IRowOperationInterceptor interceptor = new TestInterceptor();

        var row = await interceptor.FindRowAsync(
            new FindRowArgs(typeof(IdNameRow), 1, new SqlQuery(), ByIdOrSingle: true),
            TestContext.Current.CancellationToken);
        Assert.True(row.HasValue);

        var list = await interceptor.ListRowsAsync(
            new ListRowsArgs(typeof(IdNameRow), new SqlQuery()), TestContext.Current.CancellationToken);
        Assert.True(list.HasValue);

        var manipulated = await interceptor.ManipulateRowAsync(typeof(IdNameRow), 1,
            null, ExpectedRows.One, false, TestContext.Current.CancellationToken);
        Assert.Equal(1L, manipulated.Value);
    }
}
