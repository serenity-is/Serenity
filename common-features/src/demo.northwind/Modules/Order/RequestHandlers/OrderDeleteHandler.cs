using MyRow = Serenity.Demo.Northwind.OrderRow;

namespace Serenity.Demo.Northwind;

public interface IOrderDeleteHandler : IDeleteHandler<MyRow> { }

public class OrderDeleteHandler(IRequestContext context) :
    DeleteRequestHandler<MyRow>(context), IOrderDeleteHandler
{
}