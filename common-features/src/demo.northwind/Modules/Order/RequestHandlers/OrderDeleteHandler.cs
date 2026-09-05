using MyRow = Serenity.Demo.Northwind.OrderRow;

namespace Serenity.Demo.Northwind;

public interface IOrderDeleteHandler : IDeleteHandlerAsync<MyRow> { }

public class OrderDeleteHandler(IRequestContext context) :
    DeleteRequestHandlerAsync<MyRow>(context), IOrderDeleteHandler
{
}