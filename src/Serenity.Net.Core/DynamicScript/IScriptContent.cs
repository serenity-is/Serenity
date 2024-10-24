namespace Serenity.Web;

/// <summary>
/// Cached dynamic script content
/// </summary>
public interface IScriptContent
{
    /// <summary>
    /// Hash of the cached content
    /// </summary>
    public string Hash { get; }

    /// <summary>
    /// Time the cached content is generated at
    /// </summary>
    public DateTime Time { get; }

    /// <summary>
    /// Uncompressed content
    /// </summary>
    public byte[] Content { get; }

    /// <summary>
    /// Returns true if the content can be compressed
    /// </summary>
    public bool CanCompress { get; }

    /// <summary>
    /// Compressed Brotli content
    /// </summary>
    public byte[] BrotliContent { get; }

    /// <summary>
    /// Compressed Gzip content
    /// </summary>
    public byte[] CompressedContent { get; }
}