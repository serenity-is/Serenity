using MyRow = Serenity.Demo.Northwind.TerritoryRow;

namespace Serenity.Demo.Northwind;

public interface ITerritoryListHandler : IListHandlerAsync<MyRow> { }

public class TerritoryListHandler(IRequestContext context) :
    ListRequestHandlerAsync<MyRow>(context), ITerritoryListHandler
{
}