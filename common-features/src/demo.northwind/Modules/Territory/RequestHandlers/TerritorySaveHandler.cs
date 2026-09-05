using MyRow = Serenity.Demo.Northwind.TerritoryRow;

namespace Serenity.Demo.Northwind;

public interface ITerritorySaveHandler : ISaveHandlerAsync<MyRow> { }

public class TerritorySaveHandler(IRequestContext context) :
    SaveRequestHandlerAsync<MyRow>(context), ITerritorySaveHandler
{
}