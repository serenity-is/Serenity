using MyRow = Serenity.Demo.Northwind.OrderRow;

namespace Serenity.Demo.Northwind;

public interface IOrderSaveHandler : ISaveHandler<MyRow> { }

public class OrderSaveHandler(IRequestContext context) :
    SaveRequestHandler<MyRow>(context), IOrderSaveHandler
{
}