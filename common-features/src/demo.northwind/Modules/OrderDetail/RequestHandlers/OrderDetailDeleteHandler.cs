using MyRow = Serenity.Demo.Northwind.OrderDetailRow;

namespace Serenity.Demo.Northwind;

public interface IOrderDetailDeleteHandler : IDeleteHandler<MyRow> { }

public class OrderDetailDeleteHandler(IRequestContext context) :
    DeleteRequestHandler<MyRow>(context), IOrderDetailDeleteHandler
{
}