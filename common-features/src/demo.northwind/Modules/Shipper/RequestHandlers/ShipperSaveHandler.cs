using MyRow = Serenity.Demo.Northwind.ShipperRow;

namespace Serenity.Demo.Northwind;

public interface IShipperSaveHandler : ISaveHandler<MyRow> { }

public class ShipperSaveHandler(IRequestContext context) :
    SaveRequestHandler<MyRow>(context), IShipperSaveHandler
{
}