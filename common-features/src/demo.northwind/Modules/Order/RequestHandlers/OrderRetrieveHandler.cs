using MyRow = Serenity.Demo.Northwind.OrderRow;

namespace Serenity.Demo.Northwind;

public interface IOrderRetrieveHandler : IRetrieveHandler<MyRow> { }

public class OrderRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandler<MyRow>(context), IOrderRetrieveHandler
{
}