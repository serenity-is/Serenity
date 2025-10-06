using MyRow = Serenity.Demo.Northwind.NoteRow;

namespace Serenity.Demo.Northwind;

public interface INoteListHandler : IListHandler<MyRow> { }

public class NoteListHandler(IRequestContext context) :
    ListRequestHandler<MyRow>(context), INoteListHandler
{
}