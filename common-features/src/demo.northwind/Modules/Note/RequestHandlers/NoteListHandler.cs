using MyRow = Serenity.Demo.Northwind.NoteRow;

namespace Serenity.Demo.Northwind;

public interface INoteListHandler : IListHandlerAsync<MyRow> { }

public class NoteListHandler(IRequestContext context) :
    ListRequestHandlerAsync<MyRow>(context), INoteListHandler
{
}
