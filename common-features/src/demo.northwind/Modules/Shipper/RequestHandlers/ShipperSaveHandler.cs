using MyRequest = Serenity.Services.SaveRequest<Serenity.Demo.Northwind.ShipperRow>;
using MyResponse = Serenity.Services.SaveResponse;
using MyRow = Serenity.Demo.Northwind.ShipperRow;

namespace Serenity.Demo.Northwind;

public interface IShipperSaveHandler : ISaveHandler<MyRow, MyRequest, MyResponse> { }

public class ShipperSaveHandler(IRequestContext context) :
    SaveRequestHandler<MyRow, MyRequest, MyResponse>(context), IShipperSaveHandler
{
}