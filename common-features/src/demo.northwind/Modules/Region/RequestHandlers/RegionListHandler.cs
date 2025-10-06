using MyRow = Serenity.Demo.Northwind.RegionRow;

namespace Serenity.Demo.Northwind;

public interface IRegionListHandler : IListHandler<MyRow> { }

public class RegionListHandler(IRequestContext context) :
    ListRequestHandler<MyRow>(context), IRegionListHandler
{
}