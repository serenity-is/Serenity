namespace Serenity.Extensions;

/// <summary>
/// The response model for a translation list request.
/// </summary>
public class TranslationListResponse : ListResponse<TranslationItem>
{
    /// <summary>
    /// The translation keys grouped by the assembly they originate from.
    /// </summary>
    public Dictionary<string, HashSet<string>> KeysByAssembly { get; private set; } = [];
}