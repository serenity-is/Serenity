using MyRow = Serenity.Demo.Northwind.TerritoryRow;

namespace Serenity.Demo.Northwind;

public interface ITerritorySaveHandler : ISaveHandler<MyRow> { }

public class TerritorySaveHandler(IRequestContext context) :
    SaveRequestHandler<MyRow>(context), ITerritorySaveHandler
{
}