using MyRow = Serenity.Demo.Northwind.OrderRow;

namespace Serenity.Demo.Northwind;

public interface IOrderSaveHandler : ISaveHandlerAsync<MyRow> { }

public class OrderSaveHandler(IRequestContext context) :
    SaveRequestHandlerAsync<MyRow>(context), IOrderSaveHandler
{
}