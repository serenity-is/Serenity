namespace Serenity.Tests;

public class MockTextLocalizer : ITextLocalizer
{
    private readonly ILocalTextRegistry registry;
    private readonly Func<string> getLanguageId;
    private readonly Func<bool> getPending;

    public MockTextLocalizer(ILocalTextRegistry registry, Func<string> getLanguageId = null, Func<bool> getPending = null)
    {
        this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
        this.getLanguageId = getLanguageId;
        this.getPending = getPending;
    }

    public string TryGet(string key)
    {
        return registry.TryGet(getLanguageId != null ? getLanguageId() : LocalText.InvariantLanguageID,
            key, getPending != null && getPending());
    }
}