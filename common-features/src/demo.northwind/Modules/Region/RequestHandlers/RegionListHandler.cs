using MyRow = Serenity.Demo.Northwind.RegionRow;

namespace Serenity.Demo.Northwind;

public interface IRegionListHandler : IListHandlerAsync<MyRow> { }

public class RegionListHandler(IRequestContext context) :
    ListRequestHandlerAsync<MyRow>(context), IRegionListHandler
{
}