using MyRow = Serenity.Demo.Northwind.OrderDetailRow;

namespace Serenity.Demo.Northwind;

public interface IOrderDetailRetrieveHandler : IRetrieveHandler<MyRow> { }

public class OrderDetailRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandler<MyRow>(context), IOrderDetailRetrieveHandler
{
}