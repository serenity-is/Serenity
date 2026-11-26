namespace Serenity.Web;

/// <summary>
/// The loaded manifest of an <c>esm</c> folder for <see cref="EsmAccessPolicy"/>, with the
/// access results computed from it.
/// </summary>
internal class EsmRootState(EsmManifest? manifest, DateTimeOffset? lastModified, DateTime checkedAt)
{
    public EsmManifest? Manifest { get; } = manifest;
    public DateTimeOffset? LastModified { get; } = lastModified;
    public DateTime CheckedAt { get; set; } = checkedAt;
    public ConcurrentDictionary<string, EsmFileAccess> Access { get; } = new(StringComparer.OrdinalIgnoreCase);
}
