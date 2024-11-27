using MyRequest = Serenity.Services.SaveRequest<Serenity.Demo.Northwind.TerritoryRow>;
using MyResponse = Serenity.Services.SaveResponse;
using MyRow = Serenity.Demo.Northwind.TerritoryRow;

namespace Serenity.Demo.Northwind;

public interface ITerritorySaveHandler : ISaveHandler<MyRow, MyRequest, MyResponse> { }

public class TerritorySaveHandler(IRequestContext context) :
    SaveRequestHandler<MyRow, MyRequest, MyResponse>(context), ITerritorySaveHandler
{
}