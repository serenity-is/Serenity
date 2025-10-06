using MyRow = Serenity.Demo.Northwind.ShipperRow;

namespace Serenity.Demo.Northwind;

public interface IShipperListHandler : IListHandler<MyRow> { }

public class ShipperListHandler(IRequestContext context) :
    ListRequestHandler<MyRow>(context), IShipperListHandler
{
}