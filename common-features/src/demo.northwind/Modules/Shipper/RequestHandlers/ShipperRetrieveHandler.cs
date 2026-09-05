using MyRow = Serenity.Demo.Northwind.ShipperRow;

namespace Serenity.Demo.Northwind;

public interface IShipperRetrieveHandler : IRetrieveHandlerAsync<MyRow> { }

public class ShipperRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandlerAsync<MyRow>(context), IShipperRetrieveHandler
{
}