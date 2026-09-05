using MyRow = Serenity.Demo.Northwind.ShipperRow;

namespace Serenity.Demo.Northwind;

public interface IShipperListHandler : IListHandlerAsync<MyRow> { }

public class ShipperListHandler(IRequestContext context) :
    ListRequestHandlerAsync<MyRow>(context), IShipperListHandler
{
}