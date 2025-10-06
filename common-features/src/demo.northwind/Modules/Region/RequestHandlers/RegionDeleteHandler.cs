using MyRow = Serenity.Demo.Northwind.RegionRow;

namespace Serenity.Demo.Northwind;

public interface IRegionDeleteHandler : IDeleteHandler<MyRow> { }

public class RegionDeleteHandler(IRequestContext context) :
    DeleteRequestHandler<MyRow>(context), IRegionDeleteHandler
{
}