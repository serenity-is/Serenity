using MyRow = Serenity.Demo.Northwind.ShipperRow;

namespace Serenity.Demo.Northwind;

public interface IShipperRetrieveHandler : IRetrieveHandler<MyRow> { }

public class ShipperRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandler<MyRow>(context), IShipperRetrieveHandler
{
}