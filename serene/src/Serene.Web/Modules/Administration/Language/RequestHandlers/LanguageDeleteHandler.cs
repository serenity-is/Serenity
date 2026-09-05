using MyRow = Serene.Administration.LanguageRow;

namespace Serene.Administration;

public interface ILanguageDeleteHandler : IDeleteHandlerAsync<MyRow> { }

public class LanguageDeleteHandler(IRequestContext context)
    : DeleteRequestHandlerAsync<MyRow>(context), ILanguageDeleteHandler
{
}