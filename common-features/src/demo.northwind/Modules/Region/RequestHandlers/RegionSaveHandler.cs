using MyRow = Serenity.Demo.Northwind.RegionRow;

namespace Serenity.Demo.Northwind;

public interface IRegionSaveHandler : ISaveHandler<MyRow> { }

public class RegionSaveHandler(IRequestContext context) :
    SaveRequestHandler<MyRow>(context), IRegionSaveHandler
{
}