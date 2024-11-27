namespace Serenity.Extensions;

public class TranslationListResponse : ListResponse<TranslationItem>
{
    public Dictionary<string, HashSet<string>> KeysByAssembly { get; private set; } = [];
}