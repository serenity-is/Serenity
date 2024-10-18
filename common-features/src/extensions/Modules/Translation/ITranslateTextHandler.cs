namespace Serenity.Extensions;

public interface ITranslateTextHandler
{
    TranslateTextResponse Translate(TranslateTextRequest request);
}