using MyRow = Serenity.Demo.Northwind.TerritoryRow;

namespace Serenity.Demo.Northwind;

public interface ITerritoryDeleteHandler : IDeleteHandler<MyRow> { }

public class TerritoryDeleteHandler(IRequestContext context) :
    DeleteRequestHandler<MyRow>(context), ITerritoryDeleteHandler
{
}