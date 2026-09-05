using MyRow = Serenity.Demo.Northwind.OrderDetailRow;

namespace Serenity.Demo.Northwind;

public interface IOrderDetailListHandler : IListHandlerAsync<MyRow> { }

public class OrderDetailListHandler(IRequestContext context) :
    ListRequestHandlerAsync<MyRow>(context), IOrderDetailListHandler
{
}