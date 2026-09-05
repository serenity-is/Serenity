using MyRow = Serenity.Demo.Northwind.OrderDetailRow;

namespace Serenity.Demo.Northwind;

public interface IOrderDetailRetrieveHandler : IRetrieveHandlerAsync<MyRow> { }

public class OrderDetailRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandlerAsync<MyRow>(context), IOrderDetailRetrieveHandler
{
}