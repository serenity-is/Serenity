using MyRow = Serenity.Demo.Northwind.ShipperRow;

namespace Serenity.Demo.Northwind;

public interface IShipperSaveHandler : ISaveHandlerAsync<MyRow> { }

public class ShipperSaveHandler(IRequestContext context) :
    SaveRequestHandlerAsync<MyRow>(context), IShipperSaveHandler
{
}