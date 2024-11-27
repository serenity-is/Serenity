using MyRow = Serene.Administration.LanguageRow;
using MyRequest = Serenity.Services.DeleteRequest;
using MyResponse = Serenity.Services.DeleteResponse;


namespace Serene.Administration;

public interface ILanguageDeleteHandler : IDeleteHandler<MyRow, MyRequest, MyResponse> { }

public class LanguageDeleteHandler : DeleteRequestHandler<MyRow, MyRequest, MyResponse>, ILanguageDeleteHandler
{
    public LanguageDeleteHandler(IRequestContext context)
         : base(context)
    {
    }
}