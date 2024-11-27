using MyRequest = Serenity.Services.RetrieveRequest;
using MyResponse = Serenity.Services.RetrieveResponse<Serenity.Demo.Northwind.NoteRow>;
using MyRow = Serenity.Demo.Northwind.NoteRow;

namespace Serenity.Demo.Northwind;

public interface INoteRetrieveHandler : IRetrieveHandler<MyRow, MyRequest, MyResponse> { }

public class NoteRetrieveHandler(IRequestContext context) :
    RetrieveRequestHandler<MyRow, MyRequest, MyResponse>(context), INoteRetrieveHandler
{
}