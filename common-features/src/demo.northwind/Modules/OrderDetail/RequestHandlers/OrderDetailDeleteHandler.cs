using MyRow = Serenity.Demo.Northwind.OrderDetailRow;

namespace Serenity.Demo.Northwind;

public interface IOrderDetailDeleteHandler : IDeleteHandlerAsync<MyRow> { }

public class OrderDetailDeleteHandler(IRequestContext context) :
    DeleteRequestHandlerAsync<MyRow>(context), IOrderDetailDeleteHandler
{
}