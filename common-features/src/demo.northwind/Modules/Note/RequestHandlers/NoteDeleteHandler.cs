using MyRow = Serenity.Demo.Northwind.NoteRow;

namespace Serenity.Demo.Northwind;

public interface INoteDeleteHandler : IDeleteHandler<MyRow> { }

public class NoteDeleteHandler(IRequestContext context) :
    DeleteRequestHandler<MyRow>(context), INoteDeleteHandler
{
}