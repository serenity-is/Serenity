using MyRow = Serenity.Demo.Northwind.OrderRow;

namespace Serenity.Demo.Northwind;

public interface IOrderListHandler : IListHandler<MyRow, OrderListRequest, ListResponse<MyRow>> { }

public class OrderListHandler(IRequestContext context) :
    ListRequestHandler<MyRow, OrderListRequest, ListResponse<MyRow>>(context), IOrderListHandler
{
    protected override void ApplyFilters(SqlQuery query)
    {
        base.ApplyFilters(query);

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