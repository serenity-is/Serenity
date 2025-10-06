using MyRow = Serenity.Demo.Northwind.NoteRow;

namespace Serenity.Demo.Northwind;

public interface INoteRetrieveHandler : IRetrieveHandler<MyRow> { }

public class NoteRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandler<MyRow>(context), INoteRetrieveHandler
{
}