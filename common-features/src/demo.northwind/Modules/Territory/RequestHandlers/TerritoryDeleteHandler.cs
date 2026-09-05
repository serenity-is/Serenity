using MyRow = Serenity.Demo.Northwind.TerritoryRow;

namespace Serenity.Demo.Northwind;

public interface ITerritoryDeleteHandler : IDeleteHandlerAsync<MyRow> { }

public class TerritoryDeleteHandler(IRequestContext context) :
    DeleteRequestHandlerAsync<MyRow>(context), ITerritoryDeleteHandler
{
}