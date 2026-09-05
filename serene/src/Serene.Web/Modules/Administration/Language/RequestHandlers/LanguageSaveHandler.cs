using MyRow = Serene.Administration.LanguageRow;

namespace Serene.Administration;

public interface ILanguageSaveHandler : ISaveHandlerAsync<MyRow> { }

public class LanguageSaveHandler(IRequestContext context)
    : SaveRequestHandlerAsync<MyRow>(context), ILanguageSaveHandler
{
}