using MyRow = Serenity.Demo.Northwind.NoteRow;

namespace Serenity.Demo.Northwind;

public interface INoteDeleteHandler : IDeleteHandlerAsync<MyRow> { }

public class NoteDeleteHandler(IRequestContext context) :
    DeleteRequestHandlerAsync<MyRow>(context), INoteDeleteHandler
{
}
