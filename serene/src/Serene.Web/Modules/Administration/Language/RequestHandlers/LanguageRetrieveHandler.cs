using MyRow = Serene.Administration.LanguageRow;

namespace Serene.Administration;

public interface ILanguageRetrieveHandler : IRetrieveHandlerAsync<MyRow> { }

public class LanguageRetrieveHandler(IRequestContext context)
    : RetrieveRequestHandlerAsync<MyRow>(context), ILanguageRetrieveHandler
{
}