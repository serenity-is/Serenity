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
            query.Where(Criteria.Exists(
                query.SubQueryFrom(OrderDetailRow.Fields, (detailFields, detailQuery) => detailQuery
                    .Select("1")
                    .Where(
                        detailFields.OrderID == MyRow.Fields.OrderID &
                        detailFields.ProductID == Request.ProductID.Value))
                    .ToString()));
        }
    }
}
