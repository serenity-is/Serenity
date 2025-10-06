using MyRow = Serenity.Demo.Northwind.OrderDetailRow;

namespace Serenity.Demo.Northwind;

public interface IOrderDetailSaveHandler : ISaveHandler<MyRow> { }

public class OrderDetailSaveHandler(IRequestContext context) :
    SaveRequestHandler<MyRow>(context), IOrderDetailSaveHandler
{
}