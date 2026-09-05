using MyRow = Serenity.Demo.Northwind.ShipperRow;

namespace Serenity.Demo.Northwind;

public interface IShipperDeleteHandler : IDeleteHandlerAsync<MyRow> { }

public class ShipperDeleteHandler(IRequestContext context) :
    DeleteRequestHandlerAsync<MyRow>(context), IShipperDeleteHandler
{
}