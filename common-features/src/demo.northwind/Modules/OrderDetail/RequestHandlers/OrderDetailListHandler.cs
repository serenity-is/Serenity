using MyRow = Serenity.Demo.Northwind.OrderDetailRow;

namespace Serenity.Demo.Northwind;

public interface IOrderDetailListHandler : IListHandler<MyRow> { }

public class OrderDetailListHandler(IRequestContext context) :
    ListRequestHandler<MyRow>(context), IOrderDetailListHandler
{
}