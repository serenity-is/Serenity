using MyRow = Serenity.Demo.Northwind.RegionRow;

namespace Serenity.Demo.Northwind;

public interface IRegionSaveHandler : ISaveHandlerAsync<MyRow> { }

public class RegionSaveHandler(IRequestContext context) :
    SaveRequestHandlerAsync<MyRow>(context), IRegionSaveHandler
{
}