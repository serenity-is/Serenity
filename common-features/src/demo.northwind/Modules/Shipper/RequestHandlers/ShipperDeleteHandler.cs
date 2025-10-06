using MyRow = Serenity.Demo.Northwind.ShipperRow;

namespace Serenity.Demo.Northwind;

public interface IShipperDeleteHandler : IDeleteHandler<MyRow> { }

public class ShipperDeleteHandler(IRequestContext context) :
    DeleteRequestHandler<MyRow>(context), IShipperDeleteHandler
{
}