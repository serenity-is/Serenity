using MyRow = Serenity.Demo.Northwind.NoteRow;

namespace Serenity.Demo.Northwind;

public interface INoteSaveHandler : ISaveHandler<MyRow> { }

public class NoteSaveHandler(IRequestContext context) :
    SaveRequestHandler<MyRow>(context), INoteSaveHandler
{
}