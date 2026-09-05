using MyRow = Serenity.Demo.Northwind.NoteRow;

namespace Serenity.Demo.Northwind;

public interface INoteSaveHandler : ISaveHandlerAsync<MyRow> { }

public class NoteSaveHandler(IRequestContext context) :
    SaveRequestHandlerAsync<MyRow>(context), INoteSaveHandler
{
}
