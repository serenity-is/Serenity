using MyRow = Serene.Administration.LanguageRow;

namespace Serene.Administration;

public interface ILanguageListHandler : IListHandlerAsync<MyRow> { }

public class LanguageListHandler(IRequestContext context)
    : ListRequestHandlerAsync<MyRow>(context), ILanguageListHandler
{
}