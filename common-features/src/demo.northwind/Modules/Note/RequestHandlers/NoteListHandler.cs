using MyRequest = Serenity.Services.ListRequest;
using MyResponse = Serenity.Services.ListResponse<Serenity.Demo.Northwind.NoteRow>;
using MyRow = Serenity.Demo.Northwind.NoteRow;

namespace Serenity.Demo.Northwind;

public interface INoteListHandler : IListHandler<MyRow, MyRequest, MyResponse> { }

public class NoteListHandler(IRequestContext context) :
    ListRequestHandler<MyRow, MyRequest, MyResponse>(context), INoteListHandler
{
}