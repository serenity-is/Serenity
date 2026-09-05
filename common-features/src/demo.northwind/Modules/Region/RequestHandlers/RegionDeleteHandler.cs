using MyRow = Serenity.Demo.Northwind.RegionRow;

namespace Serenity.Demo.Northwind;

public interface IRegionDeleteHandler : IDeleteHandlerAsync<MyRow> { }

public class RegionDeleteHandler(IRequestContext context) :
    DeleteRequestHandlerAsync<MyRow>(context), IRegionDeleteHandler
{
}