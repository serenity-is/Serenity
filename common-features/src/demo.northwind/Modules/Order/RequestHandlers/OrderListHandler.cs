using MyRow = Serenity.Demo.Northwind.OrderRow;

namespace Serenity.Demo.Northwind;

public interface IOrderListHandler : IListHandlerAsync<MyRow, OrderListRequest, ListResponse<MyRow>> { }

public class OrderListHandler(IRequestContext context) :
    ListRequestHandlerAsync<MyRow, OrderListRequest, ListResponse<MyRow>>(context), IOrderListHandler
{
    protected override async Task ApplyFiltersAsync(SqlQuery query, CancellationToken cancellationToken = default)
    {
        await base.ApplyFiltersAsync(query, cancellationToken).ConfigureAwait(false);

        if (Request.ProductID != null)
        {
            var od = OrderDetailRow.Fields.As("od");

            query.Where(Criteria.Exists(
                query.SubQuery()
                    .Select("1")
                    .From(od)
                    .Where(
                        od.OrderID == MyRow.Fields.OrderID &
                        od.ProductID == Request.ProductID.Value)
                    .ToString()));
        }
    }
}
