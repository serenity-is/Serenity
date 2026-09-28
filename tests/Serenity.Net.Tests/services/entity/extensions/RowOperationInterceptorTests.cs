using System.Collections;

namespace Serenity.Data;

public class RowOperationInterceptorTests
{
    private sealed class TestInterceptor : IRowOperationInterceptor
    {
        public InterceptFindRowArgs? LastFindRowArgs { get; private set; }
        public InterceptListRowsArgs? LastListRowsArgs { get; private set; }
        public InterceptManipulateRowArgs? LastManipulateRowArgs { get; private set; }

        public OptionalValue<IRow> FindRow(InterceptFindRowArgs args)
        {
            LastFindRowArgs = args;
            return new(new IdNameRow());
        }

        public OptionalValue<IList> ListRows(InterceptListRowsArgs args)
        {
            LastListRowsArgs = args;
            return new(new List<IRow>());
        }

        public OptionalValue<long?> ManipulateRow(InterceptManipulateRowArgs args)
        {
            LastManipulateRowArgs = args;
            return new(1L);
        }
    }

    [Fact]
    public async Task Async_Methods_Forward_To_Sync()
    {
        IRowOperationInterceptor interceptor = new TestInterceptor();
        var testInterceptor = (TestInterceptor)interceptor;
        var cancellationToken = TestContext.Current.CancellationToken;

        var row = await interceptor.FindRowAsync(
            new InterceptFindRowArgs(typeof(IdNameRow), 1, new SqlQuery(), ByIdOrSingle: true)
            { CancellationToken = cancellationToken });
        Assert.True(row.HasValue);
        Assert.True(testInterceptor.LastFindRowArgs!.IsAsync);
        Assert.Equal(cancellationToken, testInterceptor.LastFindRowArgs.CancellationToken);

        var list = await interceptor.ListRowsAsync(
            new InterceptListRowsArgs(typeof(IdNameRow), new SqlQuery(), CountOnly: false)
            { CancellationToken = cancellationToken });
        Assert.True(list.HasValue);
        Assert.True(testInterceptor.LastListRowsArgs!.IsAsync);
        Assert.Equal(cancellationToken, testInterceptor.LastListRowsArgs.CancellationToken);

        var manipulated = await interceptor.ManipulateRowAsync(
            new InterceptManipulateRowArgs(typeof(IdNameRow), 1, null, ExpectedRows.One, GetNewId: false)
            { CancellationToken = cancellationToken });
        Assert.Equal(1L, manipulated.Value);
        Assert.True(testInterceptor.LastManipulateRowArgs!.IsAsync);
        Assert.Equal(cancellationToken, testInterceptor.LastManipulateRowArgs.CancellationToken);
    }
}
