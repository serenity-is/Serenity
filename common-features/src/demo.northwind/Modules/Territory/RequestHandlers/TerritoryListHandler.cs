using MyRow = Serenity.Demo.Northwind.TerritoryRow;

namespace Serenity.Demo.Northwind;

public interface ITerritoryListHandler : IListHandler<MyRow> { }

public class TerritoryListHandler(IRequestContext context) :
    ListRequestHandler<MyRow>(context), ITerritoryListHandler
{
}