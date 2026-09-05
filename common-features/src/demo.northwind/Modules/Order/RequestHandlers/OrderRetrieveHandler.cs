using MyRow = Serenity.Demo.Northwind.OrderRow;

namespace Serenity.Demo.Northwind;

public interface IOrderRetrieveHandler : IRetrieveHandlerAsync<MyRow> { }

public class OrderRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandlerAsync<MyRow>(context), IOrderRetrieveHandler
{
}